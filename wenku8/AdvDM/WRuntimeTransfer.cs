using System;
using System.Collections.Generic;
using System.Linq;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.IO;
using Net.Astropenguin.Linq;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;
using Net.Astropenguin.Helpers;

namespace wenku8.AdvDM
{
    using Ext;
    using Resources;
    using Settings;

    class WRuntimeTransfer : ActiveData
	{
        public readonly string ID = typeof( WRuntimeTransfer ).Name;

		public static int FailedCount = 0;

        public static int Stat_CCount = 0;
        public static int Stat_FCount = 0;

		#region DThread Handlers
		public delegate void DCycleCompleteHandler( object sender, DCycleCompleteArgs DArgs );
		public delegate void DThreadCompleteHandler( DTheradCompleteArgs DArgs );
		public delegate void DThreadUpdateHandler( DThreadUpdateArgs DArgs );

		private event DThreadCompleteHandler DThreadComplete;
		private event DThreadUpdateHandler DThreadUpdate;

        private event DCycleCompleteHandler DCycleComplete;

		public event DThreadUpdateHandler OnThreadUpdate
		{
			add
			{
				DThreadUpdate -= value;
				DThreadUpdate += value;
			}
			remove
			{
				DThreadUpdate -= value;
			}
		}
		public event DThreadCompleteHandler OnThreadComplete
		{
			add
			{
				DThreadComplete -= value;
				DThreadComplete += value;
			}
			remove
			{
				DThreadComplete -= value;
			}
		}
		public event DCycleCompleteHandler OnCycleComplete
		{
			add
			{
				DCycleComplete -= value;
				DCycleComplete += value;
			}
			remove
			{
				DCycleComplete -= value;
			}
		}
		#endregion

		private bool ResumeSession = true, IsCycleStarted = false;
		private WHTTPRequest CurrentTransfer;
		private Action<DRequestCompletedEventArgs, TransferInst> ResponseProccessor;
		private TransferInst LastThread;
		public TransferInst CurrentThread;

        private Dictionary<string, TransferInst> MemTransfers;

        public string ShortStat
        {
            get
            {
                return string.Format(
                    "P: {0} C: {1} F: {2} X:{3} Status: {4}"
                    , GetPendings().Count()
                    , Stat_CCount 
                    , Stat_FCount 
                    , GetDeactivated().Count()
                    , IsCycleStarted ? "Active" : "Inactive"
                );
            }
        }

		public bool Running { get { return ResumeSession; } }

        private const string RREG = FileLinks.ROOT_SETTING + FileLinks.ADM_RUNTIME_REG;

        public WRuntimeTransfer()
        {
            CurrentTransfer = new WHTTPRequest(
                X.Static<Func<XKey[], Uri>>
                ( XProto.WRuntimeCache, "Protocol" )
                ( null )
            );

            // Request Header
            CurrentTransfer.ContentType = "application/x-www-form-urlencoded";
            CurrentTransfer.OnRequestComplete += CurrentTransfer_OnRequestComplete;

            MemTransfers = new Dictionary<string, TransferInst>();
        }

        private IEnumerable<TransferInst> GetPendings()
        {
            return MemTransfers.Values.ToArray().Where( x => x != null && x.State == AppKeys.DM_PENDING );
        }

        private IEnumerable<TransferInst> GetDeactivated()
        {
            return MemTransfers.Values.ToArray().Where( x => x != null && x.State == AppKeys.DM_DEACTIVATED );
        }

		public void RegisterRuntimeThread( XKey[] Request, string SaveLocation, Guid DGroup, string Description, string CParam )
		{
			if ( MemTransfers.ContainsKey( SaveLocation ) )
            {
				// Thread Already exists
				Logger.Log( ID, "Request Exists", LogType.INFO );
            }
            else
			{
                MemTransfers[ SaveLocation ] = new TransferInst(
                    SaveLocation
                    , X.Call<string>( XProto.WRuntimeCache, "GetRequestUri", new object[] { Request } )
                    , DGroup
                    , Description
                    , CParam
                );

                UpdateStatus();

                if ( DThreadUpdate != null )
				{
					Worker.UIInvoke( () =>
					{
						if ( DThreadUpdate != null )
							DThreadUpdate( new DThreadUpdateArgs( SaveLocation, DGroup, Description ) );
					} );
				}
			}
		}

		public void PauseThread()
		{
			ResumeSession = false;
		}

		public void ResumeThread()
		{
			if ( !ResumeSession )
			{
				if ( !IsCycleStarted )
				{
					IsCycleStarted = ResumeSession = true;
					PushPending();
				}
				else
				{
					ResumeSession = true;
				}
			}
		}

		public bool CancelThread( string SaveLocation )
		{
			if ( CurrentThread == null || CurrentThread.ID != SaveLocation )
			{
				MemTransfers.Remove( SaveLocation );
				// WAdvRDM.Save();
				Logger.Log( ID, "Thread Canceled", LogType.INFO );
				return true;
			}
			// If it is the current thread, cannot remove
			return false;
		}

		public void Terminate()
		{
			CurrentTransfer.Stop();
			ResumeSession = false;
			CurrentThread = null;

            foreach( string id in GetPendings().Remap( x => x.ID ) )
            {
                MemTransfers.Remove( id );
            }
		}

		public void ClearFailedList()
		{
            foreach( string id in GetDeactivated().Remap( x => x.ID ) )
            {
                MemTransfers.Remove( id );
            }

            // Save
		}

		#region Thread-Chain-Cycle

		private void ActivateCurrentThread()
		{
            CurrentTransfer.UpdateProto( X.Static<Func<XKey[], Uri>>( XProto.WRuntimeCache, "Protocol" )( null ) );
			CurrentTransfer.OpenAsyncThread( CurrentThread.Request , false );
		}
		/// <summary>
		/// Start Thread Cycle.
		/// </summary>
		/// <param name="Processor">A Thread Complpte processing function, be as light as possible.</param>
		public void StartThreadCycle( Action<DRequestCompletedEventArgs, TransferInst> Processor )
		{
			// This function need to be call once only.
			if ( !IsCycleStarted )
			{
				ResponseProccessor = Processor;
				if ( ResumeSession )
				{
					IsCycleStarted = true;
					// Try to start the cycle if not started
					if ( CurrentThread == null )
					{
						PushPending();
					}
					else if ( Shared.Storage.FileExists( CurrentThread.ID ) )
					{
						// CurrentThread Exists and file exists, file saved
						StepNext();
					}
					else
					{
						// Called when app relaunched
						Logger.Log( ID, "Current Thread Activated", LogType.INFO );
						ActivateCurrentThread();
					}
				}
			}
		}

		private void CurrentTransfer_OnRequestComplete( DRequestCompletedEventArgs DArgs )
		{
			if ( CurrentThread != null )
			{
				LastThread = CurrentThread;
			}
			try
			{
                // if save failed, exception will be triggered
                ResponseProccessor(
                    DArgs.ResponseBytes.Length < 6
                    ? new DRequestCompletedEventArgs(
                        DArgs.RequestUrl
                        , DArgs.Error == null ? new Exception( "Unexpected server result" ) : DArgs.Error )
                    : DArgs
                    , LastThread );

                Stat_CCount++;
				if ( DThreadComplete != null )
				{
					// Raise evennt to UI thread
					Worker.UIInvoke( () =>
					{
						DThreadComplete( new DTheradCompleteArgs( LastThread.ID , LastThread.Group ) );
						LastThread = null;
					} );
				}
				StepNext();
			}
			catch ( Exception )
			{
                Stat_FCount++;
				if ( LastThread != null )
				{
                    // Error occured, push request back to pending list
                    LastThread.State = AppKeys.DM_PENDING;
                    LastThread.FailedCount++;

                    // Increment failed count
                    if ( 3 < LastThread.FailedCount )
                    {
                        // Failed too many times, deactivate thread
                        Logger.Log( ID, "Thread deactivated:" + LastThread.ID , LogType.INFO );
                        LastThread.State = AppKeys.DM_DEACTIVATED;
                        // XXX: Save();
                    }
				}
				PushPending();
			}
            UpdateStatus();
        }

        private void StepNext()
		{
            // Save success, remove from thread list
            MemTransfers.Remove( CurrentThread.ID );
			PushPending();
		}

		private void PushPending()
		{
            // LastThread Accessed by Event dispatcher
            CurrentThread = GetPendings().FirstOrDefault();
			if ( CurrentThread != null )
			{
				if ( ResumeSession )
				{
                    // Move Request to DM_REQUEST0.
                    CurrentThread.State = "RUNNING";
					// Start-cycle
					ActivateCurrentThread();
				}
				else
				{
					Logger.Log( ID, "Cycle Paused", LogType.INFO );
					IsCycleStarted = false;
				}
			}
			else
			{
				Logger.Log( ID, "Cycle Complete", LogType.INFO );
				// No more thread
				IsCycleStarted = false;

                try
                {
                    if( DCycleComplete != null ) DCycleComplete( this, new DCycleCompleteArgs() );
                }
                catch( Exception e )
                {
                    FailedCount++;
                    Logger.Log( ID, e.Message, LogType.ERROR );
                }
                UpdateStatus();
			}
			// Runtime Transfer, don't need to save
		}

		#endregion

        private void UpdateStatus()
        {
            Worker.UIInvoke( () => NotifyChanged( "ShortStat" ) );
        }

        internal class TransferInst
        {
            public string State = AppKeys.DM_PENDING;
            public string ID { get; private set; }

            public string Request { get; private set; }
            public Guid Group { get; private set; }

            public int FailedCount { get; set; }

            public string description { get; private set; }
            public string cParam { get; private set; }

            public TransferInst( string Location, string v, Guid dGroup, string description, string cParam )
            {
                this.ID = Location;
                this.Request = v;
                this.Group = dGroup;
                this.description = description;
                this.cParam = cParam;
            }
        }
	}
}
