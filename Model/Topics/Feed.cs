using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Net.Astropenguin.Logging;

namespace GR.Model.Topics
{
	using ListItem;
	using Resources;

	abstract class Feed
	{
		public static readonly string ID = typeof( Feed ).Name;

		protected XDocument TopicXml;
		public IEnumerable<Topic> Topics { get; protected set; }
		public string LatestTopic { get; protected set; }
		public bool IsNew { get; protected set; }

		public Feed() { }

		public Feed( string Xml )
		{
			ParseXml( Xml );
		}

		protected void ParseXml( string Xml )
		{
			try
			{
				TopicXml = XDocument.Parse( Xml );
				IEnumerable<XElement> t = TopicXml.Descendants( "topic" );

				int k, l;
				Topic[] Topics = new Topic[ l = t.Count() ];

				for ( int i = 0; i < l; i++ )
				{
					// Current Topic
					XElement cPic = t.ElementAt( i );
					IEnumerable<XElement> latest = cPic.Descendants( "latest" );
					IEnumerable<XElement> p = cPic.Descendants( "digest" );
					Digests[] d;

					// Check to see if this topic has the latest elements
					int li = 0;

					if ( 0 < latest.Count() )
					{
						li = 1;
						d = new Digests[ ( k = p.Count() ) + 1 ];
						d[ 0 ] = new Digests( Text( latest.First().Value ), latest.First().Attribute( "path" ).Value );

						LatestTopic = Text( t.Descendants( "name" ).First().Value + latest.First().Value );
						IsNew = WriteCaptionIfNew( LatestTopic );
					}
					else
					{
						d = new Digests[ k = p.Count() ];
					}

					if ( 1 < k )
					{
						for ( int j = 0; j < k; j++ )
						{
							d[ j + li ] = new Digests(
								Text( p.ElementAt( j ).Value )
								, p.ElementAt( j ).Attribute( "path" ).Value
							);
						}

						Topics[ i ] = new Topic(
							Text( cPic.Descendants( "name" ).First().Value )
							, d
							, Text( cPic.Descendants( "desc" ).First().Value )
							, i.ToString()
							, Text( ( 0 < latest.Count() ) ? latest.First().Value : p.First().Value )
						);
					}
					else if ( 0 < k )
					{
						Topics[ i ] = new Topic(
							Text( cPic.Descendants( "name" ).First().Value )
							, cPic.Descendants( "digest" ).First().Attribute( "path" ).Value
							, Text( cPic.Descendants( "desc" ).First().Value )
							, i.ToString()
							, Text( cPic.Descendants( "digest" ).First().Value )
						);
					}
				}

				this.Topics = Topics;
			}
			catch ( Exception ex )
			{
				Logger.Log( ID, ex.Message, LogType.DEBUG );
			}
		}

		virtual protected string Text( string Val ) => Val == null ? Val : Shared.Conv.Chinese.Translate( Val );

		abstract protected bool WriteCaptionIfNew( string Value );
	}
}