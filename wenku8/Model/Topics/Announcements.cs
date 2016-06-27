using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Net.Astropenguin.Logging;

namespace wenku8.Model.Topics
{
    using Resources;
    using Settings;
    using ListItem;

    class Announcements : Feed
    {
        new public static readonly string ID = typeof( Announcements ).Name;

        private const string LFile = FileLinks.ROOT_WTEXT + FileLinks.NEWS_LISTS;

        new public IEnumerable<NewsItem> Topics
        {
            get
            {
                return base.Topics.Cast<NewsItem>().OrderByDescending( x => x.TimeStamp );
            }
        }

        new protected void ParseXml( string XmlString )
        {
            try
            {
                TopicXml = XDocument.Parse( XmlString );
                IEnumerable<XElement> GUIDs = TopicXml.Descendants( "item" );

                List<Topic> Topics = base.Topics as List<Topic>;
                foreach ( XElement e in GUIDs )
                {
                    Topics.Add( TopicFromElement( e ) );
                }
            }
            catch( Exception ex )
            {
                Logger.Log( ID, "ParseXml: " + ex.Message, LogType.ERROR );
            }
        }

        public Announcements()
        {
            base.Topics = new List<Topic>();

            if ( Shared.Storage.FileExists( LFile ) )
            {
                ParseXml( Shared.Storage.GetString( LFile ) );
            }
        }

        public Announcements( string Xml )
        {
            base.Topics = new List<Topic>();

            ParseXml( Xml );
        }

        public XElement GetItem( string GUID )
        {
            if ( TopicXml == null ) return null;
            return TopicXml.Descendants( "guid" ).First( x => x.Value == GUID ).Parent;
        }

        public void MarkNew( XElement Item )
        {
            IsNew = true;
            Item.SetAttributeValue( "new", 1 );

            Topic TopicItem = TopicFromElement( Item );
            ( base.Topics as List<Topic> ).Add( TopicItem );

            if ( TopicXml == null || Item == null ) return;

            XElement channel = TopicXml.Descendants( "channel" ).First();
            channel.AddFirst( Item );
        }

        public void MaskAsRead( NewsItem Item )
        {
            XElement XItem = TopicXml.Descendants( "guid" ).First( x => x.Value == Item.Payload ).Parent;
            XAttribute NewFlag = XItem.Attribute( "new" );
            if ( NewFlag != null )
            {
                NewFlag.Remove();
                Save();
            }

            Item.FlagAsRead();
        }

        public void AllRead()
        {
            foreach( NewsItem Item in base.Topics )
            {
                Item.FlagAsRead();
            }

            foreach( XElement XItem in TopicXml.Descendants( "item" ) )
            {
                XAttribute NewFlag = XItem.Attribute( "new" );
                if ( NewFlag != null ) NewFlag.Remove();
            }

            Save();
        }

        public void Save()
        {
            Shared.Storage.WriteString( LFile, TopicXml.ToString() );
        }

        protected override bool WriteCaptionIfNew( string Value )
        {
            return false;
        }

        private Topic TopicFromElement( XElement e )
        {
            NewsItem Item = new NewsItem(
                e.Descendants( "title" ).First().Value
                , e.Descendants( "description" ).First().Value
                , e.Descendants( "link" ).First().Value
                , e.Descendants( "guid" ).First().Value
                , e.Descendants( "pubDate" ).First().Value
            );

            if( e.Attribute( "new" ) != null )
            {
                IsNew = true;
                Item.FlagAsNew();
            }

            return Item;
        }

    }
}
