using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace wenku8.Model.Book
{
    using Resources;
    using Settings;

    sealed class VolumesInfo
    {
        public static readonly string ID = typeof( VolumesInfo ).Name;
        public string[] VolTitles;
        public string[] vids;
        public string[][] cids;
        public string[][] EpTitles;

        public string BookId { get; private set; }

        public Volume[] VolRef { get; private set; }

        public VolumesInfo( BookItem b )
        {
            BookId = b.Id;
            if ( b.IsLocal() || b.IsSpider() )
            {
                ParseByVolumes( VolRef = b.GetVolumes() );
                return;
            }

            XDocument xml;
            try
            {
                xml = XDocument.Parse( Shared.Storage.GetString( b.TOCPath ) );
            }
            catch ( Exception )
            {
                VolTitles = null;
                vids = null;
                cids = null;
                EpTitles = null;
                return;
            }
            // TOC Content XML
            IEnumerable<XElement> vols = xml.Descendants( "volume" );
            int v = vols.Count();
            VolTitles = new string[ v ];
            vids = new string[ v ];

            // V-cid and V-Ep List
            cids = new string[ v ][];
            EpTitles = new string[ v ][];

            // Step over the volumes
            for ( int i = 0; i < v; i++ )
            {
                // Volume id
                vids[ i ] = vols.ElementAt( i ).Attribute( AppKeys.GLOBAL_VID ).Value;

                // Store the volume Title
                VolTitles[ i ] = vols.ElementAt( i ).Nodes().OfType<XText>().First().Value;

                IEnumerable<XElement> chs = vols.ElementAt( i ).Descendants( "chapter" );
                int c = chs.Count();

                // V-cid and V-Ep List
                string[] uCid = new string[ c ];
                string[] uETitles = new string[ c ];

                for ( int j = 0; j < c; j++ )
                {
                    // Stores Episode Name
                    uCid[ j ] = chs.ElementAt( j ).Attribute( AppKeys.GLOBAL_CID ).Value;
                    uETitles[ j ] = chs.ElementAt( j ).Value;
                }
                // Set up index
                cids[ i ] = uCid;
                EpTitles[ i ] = uETitles;
            }
        }

        private void ParseByVolumes( Volume[] Vols )
        {
            int l = Vols.Length;
            vids = new string[ l ];
            cids = new string[ l ][];
            VolTitles = new string[ l ];
            EpTitles = new string[ l ][];

            for ( int i = 0; i < l; i++ )
            {
                Volume V = Vols[ i ];
                VolTitles[ i ] = V.VolumeTitle;
                vids[ i ] = V.vid;

                Chapter[] Chs = V.ChapterList;

                int k = Chs.Length;

                cids[ i ] = new string[ k ];
                EpTitles[ i ] = new string[ k ];

                for ( int j = 0; j < k; j++ )
                {
                    Chapter C = Chs[ j ];
                    cids[ i ][ j ] = C.cid;
                    EpTitles[ i ][ j ] = C.ChapterTitle;
                }
            }
        }
    }
}