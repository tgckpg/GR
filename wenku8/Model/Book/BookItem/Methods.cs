using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Windows.UI.Xaml.Media.Imaging;

using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;

namespace wenku8.Model.Book
{
    using Resources;
    using Settings;

    partial class BookItem
    {
        public static string TypeName( BookInfo InfoType )
        {
            return Res.Text( InfoType.ToString() );
        }

        public bool ParseXml( string xml )
        {
            try
            {
                XDocument BookmetaXml = XDocument.Parse( xml );
                IEnumerable<XElement> metadata = BookmetaXml.FirstNode.Document.Descendants( "data" );
                foreach ( XElement md in metadata )
                {
                    ReadParam( md.GetXValue( "name" ), md.GetXValue( "value" ), md.Value );
                }
            }
            catch ( Exception )
            {
                return false;
            }
            return true;
        }

        public bool ReadParam( string Name, string Value, string CDATA = null )
        {
            if ( CDATA == null ) CDATA = Value;

            switch ( Name )
            {
                case AppKeys.XML_BINF_TITLE: Title = CDATA; return true;
                case AppKeys.XML_BINF_AUTHOR: Author = Value; return true;
                case AppKeys.XML_BMTA_PUSHCNT: PushCount = Value; return true;
                case AppKeys.XML_BMTA_THITCNT: TotalHitCount = Value; return true;
                case AppKeys.XML_BMTA_DHITCNT: TodayHitCount = Value; return true;
                case AppKeys.XML_BMTA_FAVCNT: FavCount = Value; return true;
                case AppKeys.XML_BMTA_LSECTION: LatestSection = CDATA; return true;
                case AppKeys.XML_BINF_INTROPRV: Description = CDATA; return true;
                case AppKeys.BINF_INTRO: Intro = Value; return true;
                case AppKeys.BINF_ORGURL: OriginalUrl = Value; return true;
                // Special for spider, push content properties
                case AppKeys.BINF_OTHERS: Others.Add( Value ); return true;

                case AppKeys.BINF_COVER:
                    try
                    {
                        new Uri( Value );
                        CoverSrcUrl = Value;
                        return true;
                    }
                    catch ( Exception )
                    {

                    }
                    return false;
                case AppKeys.BINF_PRESS:
                case AppKeys.XML_BMTA_PRESSID:
                    Press = Value;
                    return true;
                case AppKeys.BINF_STATUS:
                case AppKeys.XML_BINF_BSTATUS:
                    Status = Value;
                    return true;
                case AppKeys.BINF_LENGTH:
                case AppKeys.XML_BMTA_BLENGTH:
                    Length = Value;
                    return true;
                case AppKeys.BINF_DATE:
                case AppKeys.XML_BINF_LUPDATE:
                    RecentUpdate = Value;
                    return true;
            }

            return false;
        }

        virtual public Volume[] GetVolumes()
        {
            // Get Cached XML
            XDocument xml;
            try
            {
                if ( doc != null )
                {
                    return doc.GetVolumes();
                }

                xml = XDocument.Parse( Shared.Storage.GetString( TOCPath ) );
            }
            catch ( Exception ex )
            {
                Logger.Log( ID, ex.Message, LogType.ERROR );

                if ( Shared.Storage.FileExists( TOCPath ) )
                {
                    Logger.Log( ID, Shared.Storage.GetString( TOCPath ), LogType.INFO );
                }
                else
                {
                    Logger.Log( ID, "File Not Found: " + TOCPath, LogType.INFO );
                }

                // Remove the corrupted file
                Shared.Storage.DeleteFile( TOCPath );
                return new Volume[0];
            }

            // Initialize Volume List
            IEnumerable<XElement> Collection = xml.Document.Descendants( "volume" );
            int l = Collection.Count();

            Volume[] vList = new Volume[ l ];
            Volume p = null;

            for ( int i = 0; i < l; i++ )
            {
                XElement v = Collection.ElementAt( i );
                IEnumerable<XElement> chapterList = v.Descendants( "chapter" );

                int k;
                string vid = v.Attribute( AppKeys.GLOBAL_VID ).Value;
                Chapter[] cList = new Chapter[ k = chapterList.Count() ];

                for ( int j = 0; j < k; j++ )
                {
                    XElement c = chapterList.ElementAt( j );
                    cList[ j ] = new Chapter( c.Value, Id, vid, c.Attribute( AppKeys.GLOBAL_CID ).Value );
                }

                p = vList[ i ] = new Volume( vid, false, v.Nodes().OfType<XText>().First().Value, cList );
            }

            if ( p != null ) p.VolumeTag = true;

            return vList;
        }

        virtual public void SaveInfo( XRegistry XReg )
        {
            XParameter Param = XReg.Parameter( "METADATA" );
            if( Param == null ) Param = new XParameter( "METADATA" );

            Param.SetValue( new XKey[]
            {
                new XKey( "Title", Title )
                , new XKey( "Author", AuthorRaw )
                , new XKey( "RecentUpdate", RecentUpdateRaw )
                , new XKey( "TotalHitCount", TotalHitCountRaw )
                , new XKey( "TodayHitCount", TodayHitCountRaw )
                , new XKey( "PushCount", PushCountRaw )
                , new XKey( "FavCount", FavCountRaw )
                , new XKey( "Length", LengthRaw )
                , new XKey( "LatestSection", LatestSection )
                , new XKey( "Press", PressRaw )
                , new XKey( "Intro", IntroRaw )
                , new XKey( "OriginalUrl", OriginalUrl )
            } );

            int i = 0;
            foreach( string S in Others )
            {
                XParameter OtherParam = new XParameter( "others" );
                OtherParam.Id += i++;
                OtherParam.SetValue( new XKey( "other", S ) );
                Param.SetParameter( OtherParam );
            }

            XReg.SetParameter( Param );
            XReg.Save();
        }

        virtual public void ReadInfo( XRegistry XReg )
        {
            XParameter Param = XReg.Parameter( "METADATA" );
            if ( Param == null ) return;

            Title = Param.GetValue( "Title" );
            Author = Param.GetValue( "Author" );
            RecentUpdate = Param.GetValue( "RecentUpdate" );
            TotalHitCount = Param.GetValue( "TotalHitCount" );
            TodayHitCount = Param.GetValue( "TodayHitCount" );
            PushCount = Param.GetValue( "PushCount" );
            FavCount = Param.GetValue( "FavCount" );
            Length = Param.GetValue( "Length" );
            LatestSection = Param.GetValue( "LatestSection" );
            Press = Param.GetValue( "Press" );
            Intro = Param.GetValue( "Intro" );
            OriginalUrl = Param.GetValue( "OriginalUrl" );

            XParameter[] OtherParams = Param.Parameters( "others" );
            foreach ( XParameter OtherParam in OtherParams )
            {
                Others.Add( OtherParam.GetValue( "other" ) );
            }
        }

        public void Update( BookItem B )
        {
            Id = B.Id;
            Title = B.Title;
            Author = B.Author;
            RecentUpdate = B.RecentUpdate;
            TotalHitCount = B.TotalHitCount;
            TodayHitCount = B.TodayHitCount;
            PushCount = B.PushCount;
            FavCount = B.FavCount;
            Length = B.Length;
            LatestSection = B.LatestSection;
            Press = B.Press;
            Intro = B.Intro;
            OriginalUrl = B.OriginalUrl;
            Others = B.Others;
        }

        public void CoverUpdate()
        {
            NotifyChanged( "CoverStream" );
        }

        static public BookItem DummyBook()
        {
            BookItem Book = new BookItem();
            Book.Title = "Dummy title";
            Book.Author = "Elizabeth";
            Book.RecentUpdate = "1999-12-01";
            Book.TotalHitCount = "94217589";
            Book.TodayHitCount = "123985";
            Book.PushCount = "20300";
            Book.FavCount = "12039";
            Book.Length = "13902919";
            Book.LatestSection = "The last savior";
            Book.Press = "Good Press";
            Book.Intro = "Ducimus architecto qui sit sint odit ut.Nemo dolor minima sapiente. In reprehenderit qui voluptas voluptatibus.In in at voluptatem qui et dolor.Vitae natus consequatur autem sit autem. Enim asperiores quis soluta enim quos eveniet nobis qui."
                + "\nEum eos sapiente voluptatem. Expedita hic at pariatur repellat.Praesentium dolorem eos quasi voluptatibus optio distinctio ea. Ea modi qui quam sapiente.Debitis enim facere odit dolor impedit. Tempore et quia fugiat hic atque nostrum neque earum."
                + "\nVitae consequuntur ducimus aut dolore repellat sint.Ab quos dolores facere. Et sit ea rerum aut minima. Fuga sequi iure sunt tempore quia error dolorem. Nulla modi distinctio sit corrupti et et omnis laboriosam.Qui est ratione nesciunt et officia.";

            Book.OriginalUrl = "https://google.com/";

            Book.Others.Add( "Other 1" );
            Book.Others.Add( "Other 2" );
            Book.Others.Add( "Other 3" );

            return Book;
        }

        private string DisplayString( string Raw, BookInfo InfType, string Suffix = "" )
        {
            return string.IsNullOrEmpty( Raw ) ? "" : ( TypeName( InfType ) + ": " + Raw + Suffix );
        }

        private async void TrySetSource()
        {
            if ( _Cover == null ) return;
            using ( CoverStream )
            {
                if ( CoverStream == null ) return;

                _Cover = new BitmapImage();
                await _Cover.SetSourceAsync( CoverStream.AsRandomAccessStream() );

                NotifyChanged( "Cover" );
            }
        }

    }
}