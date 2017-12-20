using System;
using System.Linq;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Logging;

namespace GR.Model.Book
{
	public sealed class EpisodeStepper : ActiveData
	{
		public static readonly string ID = typeof( EpisodeStepper ).Name;

		private int VStepper = 0;
		private int EStepper = 0;

		private VolumesInfo VInfo;

		internal EpisodeStepper( VolumesInfo VInfo )
		{
			this.VInfo = VInfo;
		}

		internal bool SetCurrentPosition( Chapter C, bool VolHead = false )
		{
			string Id = C.cid;

			int i = Array.IndexOf( VInfo.vids, C.vid );
			if ( i == -1 ) return false;

			VStepper = i;
			EStepper = 0;

			if ( VolHead ) return false;

			while ( VStepper < VInfo.cids.Length )
			{
				while ( EStepper < VInfo.cids[ VStepper ].Length )
				{
					if ( VInfo.cids[ VStepper ][ EStepper ] == Id )
					{
						return true;
					}
					EStepper++;
				}
				EStepper = 0;
				VStepper++;
			}

			return false;
		}

		public EpisodeStepper Virtual()
		{
			EpisodeStepper ES = new EpisodeStepper( VInfo );
			ES.VStepper = VStepper;
			ES.EStepper = EStepper;
			return ES;
		}

		public void Rewind()
		{
			VStepper = 0;
			EStepper = 0;
		}

		internal Chapter Chapter
		{
			get
			{
				if ( VInfo.VolRef != null )
				{
					return VInfo
						.VolRef.First( x => x.vid == Vid )
						.ChapterList.First( x => x.cid == Cid )
						;
				}

				return new Chapter( EpTitle, VInfo.BookId, Vid, Cid );
			}
		}

		public string Cid { get { return VInfo.cids[ VStepper ][ EStepper ]; } }
		public string Vid { get { return VInfo.vids[ VStepper ]; } }
		public string EpTitle { get { return VInfo.EpTitles[ VStepper ][ EStepper ]; } }
		public string VolTitle { get { return VInfo.VolTitles[ VStepper ]; } }

		public string NextVolTitle
		{
			get
			{
				if ( VolAvailable( VStepper + 1 ) )
					return VInfo.VolTitles[ VStepper + 1 ];
				return "";
			}
		}

		public string PrevVolTitle
		{
			get
			{
				if ( VolAvailable( VStepper - 1 ) )
					return VInfo.VolTitles[ VStepper - 1 ];
				return "";
			}
		}

		public bool StepPrevVol()
		{
			if ( PrevVolAvaible() )
			{
				EStepper = 0;
				VStepper--;

				NotifyChanged( "Chapter" );
				return true;
			}
			return false;
		}

		public bool StepNextVol()
		{
			if ( NextVolAvaible() )
			{
				EStepper = 0;
				VStepper++;

				NotifyChanged( "Chapter" );
				return true;
			}
			return false;
		}

		public bool NextVolAvaible()
		{
			int i = 0;
			if ( VolAvailable( VStepper + ( ++i ) ) )
			{
				while ( !EpAvailable( 0, VStepper + i ) && VolAvailable( VStepper + ( ++i ) ) ) ;
				return EpAvailable( 0, VStepper + i );
			}

			return false;
		}

		public bool PrevVolAvaible()
		{
			int i = 0;
			if ( VolAvailable( VStepper - ( ++i ) ) )
			{
				while ( !EpAvailable( 0, VStepper - i ) && VolAvailable( VStepper - ( ++i ) ) ) ;
				return EpAvailable( 0, VStepper - i );
			}

			return false;
		}

		public bool StepNext()
		{
			if ( EpAvailable( ++EStepper ) )
			{
				NotifyChanged( "Chapter" );
				return true;
			}
			if ( !VolAvailable( ++VStepper ) ) return false;

			EStepper = 0;

			if ( VInfo.cids[ VStepper ].Length == 0 )
			{
				Logger.Log(
					ID
					, string.Format( "Volume \"{0}\" has no Episodes, skipping", VolTitle )
					, LogType.WARNING
				);
				return StepNext();
			}

			NotifyChanged( "Chapter" );

			return true;
		}

		public bool StepPrev()
		{
			if ( EpAvailable( --EStepper ) )
			{
				NotifyChanged( "Chapter" );
				return true;
			}
			if ( !VolAvailable( --VStepper ) ) return false;

			EStepper = VInfo.cids[ VStepper ].Length - 1;

			if ( EStepper < 0 )
			{
				Logger.Log(
					ID
					, string.Format( "Volume \"{0}\" has no Episodes, skipping", VolTitle )
					, LogType.WARNING
				);
				return StepPrev();
			}

			NotifyChanged( "Chapter" );

			return true;
		}

		private bool EpAvailable( int ex )
		{
			return EpAvailable( ex, VStepper );
		}

		private bool EpAvailable( int ex, int v )
		{
			return -1 < ex && VolAvailable( VStepper ) && ex < VInfo.cids[ v ].Length;
		}

		private bool VolAvailable( int vx )
		{
			return -1 < vx && vx < VInfo.cids.Length;
		}

		public bool NextStepAvailable()
		{
			if ( VInfo.cids == null ) return false;

			if ( VolAvailable( VStepper ) )
			{
				if ( EpAvailable( EStepper + 1 ) )
				{
					return true;
				}
				else
				{
					int i = 0;
					for ( int j = 1; VolAvailable( VStepper + j ); j++ )
					{
						while ( EpAvailable( i++, VStepper + j ) ) return true;
						i = 0;
					}
				}
			}

			return false;
		}

		public bool PrevStepAvailable()
		{
			if ( 0 < EStepper )
				return true;
			else if ( 0 < VStepper )
				return true;
			return false;
		}

		/*
		public string GetCorresVid( string cid )
		{
			for ( int i = 0; i < VInfo.vids.Length; i ++ )
			{
				string vid = VInfo.vids[ i ];
				string[] cids = VInfo.cids[ i ];

				if ( cids.Contains( cid ) ) return vid;
			}

			return null;
		}
		*/
	}
}