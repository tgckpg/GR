using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;
using Net.Astropenguin.Messaging;

namespace wenku8.Model.Book
{
    using Config;
    using Settings;
    using Resources;

    class LocalTextDocument
    {
        public static readonly string ID = typeof( LocalTextDocument ).Name;

        public string Title { get; private set; }
        public string Id { get; private set; }
        public string MetaLocation { get { return FileLinks.ROOT_LOCAL_VOL + Id + "/METADATA.xml"; } }

        public bool IsValid { get; private set; }

        private List<TextEpisode> Episodes;
        private List<TextVolume> Volumes;

        private XRegistry BookReg;
        public LocalTextDocument( string id )
        {
            this.Id = id;
            BookReg = new XRegistry( "<BookMeta />", MetaLocation );

            TryGetInformation();
        }

        private void TryGetInformation()
        {
            XParameter Param = BookReg.Parameter( AppKeys.GLOBAL_META );
            if( Param != null )
            {
                Title = Param.GetValue( AppKeys.GLOBAL_NAME );
                IsValid = true;
            }
        }

        public async static Task<LocalTextDocument> ParseAsync( string aid, string doc )
        {
            if( Properties.LANGUAGE_TRADITIONAL )
            {
                MessageBus.SendUI( typeof( ListItem.LocalBook ), "Translating ...", aid );
                await Task.Run( () => doc = doc.ToCTrad() );
            }

            try
            {
                LocalTextDocument TDoc = new LocalTextDocument( aid );
                TDoc.Episodes = new List<TextEpisode>();

                string[] lines = doc.Split( new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );

                int l = lines.Length - 2;

                // Filter unecessary line break and spaces
                string s;
                TDoc.Title = lines[ 1 ];
                TextEpisode Ep = null;
                for( int i = 2; i < l;  i ++ )
                {
                    s = lines[ i ];
                    if ( 3 < s.Length )
                    {
                        if ( !( s[ 0 ] == s[ 1 ] && s[ 1 ] == s[ 2 ] && s[ 2 ] == s[ 3 ] && s[ 3 ] == ' ' ) )
                        {
                            if ( Ep != null )
                            {
                                TDoc.Episodes.Add( Ep );
                            }
                            Ep = new TextEpisode( TDoc.Id, s.Trim() );
                            continue;
                        }
                    }

                    if ( Ep == null ) throw new Exception( "Invalid leading paragraph" );

                    s = s.Trim();
                    if ( !string.IsNullOrEmpty( s ) )
                    {
                        Ep.Push( s );
                    }
                }

                MessageBus.SendUI( typeof( ListItem.LocalBook ), "Analyzing ...", aid );
                await TDoc.GuessVolTitle();
                // Write Chapter Content
                // Shared.Storage.WriteString( path, content );

                return TDoc;
            }
            catch ( Exception ex )
            {
                Logger.Log( ID, ex.Message, LogType.ERROR );
            }

            throw new FormatException();
        }

        public async Task Save()
        {
            foreach( TextVolume Vol in Volumes )
            {
                BookReg.SetParameter( Vol.id, new XKey[] {
                    new XKey( AppKeys.GLOBAL_VID, true )
                    , new XKey( AppKeys.GLOBAL_NAME, Vol.Title )
                } );

                await Vol.Save();
            }

            BookReg.SetParameter( AppKeys.GLOBAL_META, new XKey( AppKeys.GLOBAL_NAME, Title ) );

            MessageBus.SendUI( typeof( ListItem.LocalBook ), "Saving Metadata... ", Id );
            BookReg.Save();
        }

        private async Task GuessVolTitle()
        {
            Logger.Log( ID, "Guessing Volumes for: " + Title, LogType.DEBUG );
            Volumes = new List<TextVolume>();

            IEnumerable<TextEpisode> OEps = Episodes.Where( x => true );
            while ( 0 < OEps.Count() )
            {
                TextEpisode First = OEps.First();
                int i = 0; int j = 0; int Taken = 0;
                IEnumerable<TextEpisode> Eps = OEps.Where( x => true );
                while ( true )
                {
                    char RefS = Eps.First( x => i < x.Title.Length ).Title[ i ];

                    int LongestTitle = 0;
                    foreach ( TextEpisode Ep in Eps )
                    {
                        if ( LongestTitle < Ep.Title.Length )
                        {
                            LongestTitle = Ep.Title.Length;
                        }

                        if ( Ep.Title.Length <= i ) continue;

                        if ( !( i < Ep.Title.Length ) || RefS != Ep.Title[ i ] )
                            break;
                        j++;
                    }

                    if ( 1 < j && i < LongestTitle )
                    {
                        Eps = Eps.Take( Taken = j );
                        i++;
                    }
                    else
                    {
                        if ( 0 < Taken )
                        {
                            Volumes.Add( ProcessVolume( Id, Eps, i ) );
                            OEps = OEps.Skip( Eps.Count() );
                        }
                        else
                        {
                            Volumes.Add( ProcessVolume( Id, OEps.Take( 1 ), i ) );
                            OEps = OEps.Skip( 1 );
                        }
                        Taken = 0;
                        break;
                    }
                    j = 0;
                }

                Eps = OEps.Where( x => true );

                await Task.Delay( 100 );
            }
        }

        private TextVolume ProcessVolume( string aid, IEnumerable<TextEpisode> VolGroup, int i )
        {
            string VolTitle = VolGroup.First().Title;
            if( 0 < i ) VolTitle = VolTitle.Substring( 0, i );

            Logger.Log( ID, "Guess this is a Volume: " + VolTitle, LogType.DEBUG );
            MessageBus.SendUI( typeof( ListItem.LocalBook ), VolTitle, aid );

            return new TextVolume( aid, VolTitle, VolGroup );
        }

        public Volume[] GetVolumes()
        {
            XParameter[] Params = BookReg.Parameters( AppKeys.GLOBAL_VID );

            List<Volume> Vols = new List<Volume>();
            foreach( XParameter Param in Params )
            {
                TextVolume TVol = new TextVolume( Id, Param.Id );
                Vols.Add( new Volume( Param.Id, false, TVol.Title, TVol.GetChapters() ) );
            }

            return Vols.ToArray();
        }
    }
}
