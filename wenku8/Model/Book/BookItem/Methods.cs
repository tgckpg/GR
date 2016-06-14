﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
                    catch( Exception )
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

        public void SaveInfo( XRegistry XReg )
        {
            XParameter Param = new XParameter( "METADATA" );
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
                OtherParam.ID += i++;
                OtherParam.SetValue( new XKey( "other", S ) );
                Param.SetParameter( OtherParam );
            }

            XReg.SetParameter( Param );
            XReg.Save();
        }

        public void ReadInfo( XRegistry XReg )
        {
            XParameter Param = XReg.GetParameter( "METADATA" );
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

            XParameter[] OtherParams = Param.GetParametersWithKey( "others" );
            foreach ( XParameter OtherParam in OtherParams )
            {
                Others.Add( OtherParam.GetValue( "other" ) );
            }
        }
    }
}
