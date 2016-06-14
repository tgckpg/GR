using System;

using Net.Astropenguin.Logging;
using System.Linq;

namespace wenku8.Model.Book
{
	class EpisodeStepper
	{
        public static readonly string ID = typeof( EpisodeStepper ).Name;

		private int VStepper = 0;
		private int EStepper = 0;

        protected VolumesInfo VInfo;

		public EpisodeStepper( VolumesInfo VInfo )
		{
            this.VInfo = VInfo;
		}

        public bool SetCurrentPosition( Chapter C, bool VolHead = false )
        {
            string Id = C.cid;

            if ( VolHead )
            {
                int i = Array.IndexOf( VInfo.vids, C.vid );

                if ( i != -1 )
                {
                    VStepper = i;
                    EStepper = 0;
                }

                return false;
            }

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

        public void Rewind()
        {
            VStepper = 0;
            EStepper = 0;
        }

        public Chapter Chapter
        {
            get
            {
                if( VInfo.VolRef != null )
                {
                    return VInfo
                        .VolRef.First( x => x.vid == currentVid )
                        .ChapterList.First( x => x.cid == currentCid )
                        ;
                }

                return new Chapter( currentEpTitle, VInfo.BookId, currentVid, currentCid );
            }
        }

		public string currentCid
		{
			get
			{
				return VInfo.cids[VStepper][EStepper];
			}
		}

		public string currentVid
		{
			get
			{
				return VInfo.vids[VStepper];
			}
		}

		public string currentEpTitle
		{
			get
			{
				return VInfo.EpTitles[VStepper][EStepper];
			}
		}

		public string CurrentVolTitle
		{
			get
			{
				return VInfo.VolTitles[VStepper];
			}
		}

        public bool stepNext()
        {
            if ( EpAvailable( ++EStepper ) )
            {
                return true;
            }

            if ( !VolAvailable( ++VStepper ) ) return false;

            EStepper = 0;

            // This Volume is empty, step Next
            if ( VInfo.cids[ VStepper ].Length == 0 )
            {
                Logger.Log(
                    ID
                    , string.Format( "Volume \"{0}\" has no Episodes, skipping", CurrentVolTitle )
                    , LogType.WARNING
                );
                return stepNext();
            }

            return true;
        }

        private bool EpAvailable( int ex )
        {
            return VolAvailable( VStepper ) && ex < VInfo.cids[ VStepper ].Length;
        }

        private bool VolAvailable( int vx )
        {
            return vx < VInfo.cids.Length;
        }

        public bool NextStepAvailable()
        {
            if ( VInfo.cids == null ) return false;

            if ( VolAvailable( VStepper ) )
            {
                if ( EpAvailable( EStepper ) )
                {
                    return true;
                }

                return stepNext();
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

		public bool stepPrev()
		{
			if ( 0 < EStepper )
			{
				EStepper--;
				return true;
			}
			else if ( 0 < VStepper )
			{
				VStepper--;
				EStepper = VInfo.cids[VStepper].Length - 1;
				return true;
			}
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