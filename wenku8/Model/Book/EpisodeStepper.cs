using System;

using Net.Astropenguin.Logging;
using System.Linq;

namespace wenku8.Model.Book
{
    public sealed class EpisodeStepper
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

        public bool StepNext()
        {
            if ( EpAvailable( ++EStepper ) ) return true;
            if ( !VolAvailable( ++VStepper ) ) return false;

            EStepper = 0;

            // This Volume is empty, step Next
            if ( VInfo.cids[ VStepper ].Length == 0 )
            {
                Logger.Log(
                    ID
                    , string.Format( "Volume \"{0}\" has no Episodes, skipping", VolTitle )
                    , LogType.WARNING
                );
                return StepNext();
            }

            return true;
        }

        private bool EpAvailable( int ex )
        {
            return -1 < ex && VolAvailable( VStepper ) && ex < VInfo.cids[ VStepper ].Length;
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
                if ( EpAvailable( EStepper ) )
                {
                    return true;
                }

                return StepNext();
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

        public bool StepPrev()
        {
            if ( 0 < EStepper )
            {
                EStepper--;
                return true;
            }
            else if ( 0 < VStepper )
            {
                VStepper--;
                EStepper = VInfo.cids[ VStepper ].Length - 1;
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