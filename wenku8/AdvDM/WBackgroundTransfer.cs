using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;

using Net.Astropenguin.IO;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;

namespace wenku8.AdvDM
{
    using Resources;
    using Settings;

    class WBackgroundTransfer
	{
        public static readonly string ID = typeof( WBackgroundTransfer ).Name;

        #region DThread Handlers
        public delegate void DThreadCompleteHandler( DTheradCompleteArgs DArgs );
		public delegate void DThreadProgressHandler( DThreadProgressArgs DArgs );
		public delegate void DThreadUpdateHandler( DThreadUpdateArgs DArgs );

		private event DThreadCompleteHandler DThreadComplete;
		private event DThreadProgressHandler DThreadProgress;
		private event DThreadUpdateHandler DThreadUpdate;

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
		public event DThreadProgressHandler OnThreadProgress
		{
			add
			{
				DThreadProgress -= value;
				DThreadProgress += value;
			}
			remove
			{
				DThreadProgress -= value;
			}
		}
		#endregion

		private const int ThreadLimit = 3
		, QueueLimit = 64;

		private static XRegistry WAdvDM;

        public WBackgroundTransfer()
        {
			WAdvDM = new XRegistry( AppKeys.AdvDM_FXML, AppKeys.DREG );
        }

        public async Task<Guid> RegisterImage( string url, string saveLocation )
        {
            try
            {
                BackgroundDownloader Downloader = new BackgroundDownloader();
                DownloadOperation Download = Downloader.CreateDownload( new Uri( url ), await Shared.Storage.GetImage( saveLocation ) );
                var j = HandleDownloadAsync( Download, saveLocation );

                return Download.Guid;
            }
            catch( Exception ex )
            {
                Logger.Log( ID, ex.Message, LogType.WARNING );
            }

            return Guid.Empty;
        }

        private async Task HandleDownloadAsync( DownloadOperation Download, string saveLocation )
        {
            try
            {
                Logger.Log( ID, "Running: " + Download.Guid, LogType.DEBUG );

                Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>( DownloadProgress );

                // Start
                await Download.StartAsync().AsTask( progressCallback );
                // await Download.AttachAsync().AsTask( progressCallback );

                ResponseInformation Response = Download.GetResponseInformation();

                Logger.Log( ID, string.Format( "Completed: {0}, Status Code: {1}", Download.Guid, Response.StatusCode ), LogType.DEBUG );

                if( DThreadComplete != null )
                    DThreadComplete( new DTheradCompleteArgs( saveLocation, Download.Guid ) );
            }
            catch ( TaskCanceledException )
            {
                Logger.Log( ID, "Canceled: " + Download.Guid, LogType.INFO );
            }
            catch ( Exception ex )
            {
                Logger.Log( ID, ex.Message, LogType.ERROR );
            }
        }

        private void DownloadProgress( DownloadOperation obj )
        {
            if( obj.Progress.TotalBytesToReceive == 0 )
            {
                Logger.Log( ID, string.Format( "{0} Bytes Received", obj.Progress.BytesReceived ), LogType.INFO );
            }
            else
            {
                Logger.Log( ID, string.Format( "{0}%", obj.Progress.BytesReceived / obj.Progress.TotalBytesToReceive * 100 ), LogType.INFO );
            }
        }

        private bool QueueAvailable()
        {
            return ( WAdvDM.CountParametersWithKey( AppKeys.DM_PENDING ) < QueueLimit );
        }

        public bool RequestRegistered( string SaveLocation )
        {
            XParameter p = WAdvDM.GetParameter( SaveLocation );
            return ( p != null && ( p.GetValue( AppKeys.DM_PENDING ) != null || p.GetValue( AppKeys.DM_REQUESTID ) != null ) );
        }

	}
}
