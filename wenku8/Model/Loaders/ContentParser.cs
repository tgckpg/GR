using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Logging;

namespace wenku8.Model.Loaders
{
    using Book;
    using Resources;
    using Text;

    sealed class ContentParser
    {
        public static readonly string ID = typeof( ContentParser ).Name;

        public async Task OrganizeBookContent( string e, Chapter C )
        {
            await Task.Run(
                () => OrganizeBookContent( e, C.ChapterPath, C.IllustrationPath )
            );
        }

        /// <summary>
        /// Content Preprocessing
        /// </summary>
        /// <param name="e"> Content </param>
        /// <param name="path"> Save Location </param>
        /// <param name="illspath"> Illustraction Location </param>
        public void OrganizeBookContent( string e, string path, string illspath )
        {
            /** WARNING: DO NOT MODIFY THIS LOGIC AS IT WILL MESS WITH THE BOOK ANCHOR **/
            try
            {
                string content = "";
                string[] paragraphs = e.Split( new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );
                // Filter unecessary line break and spaces
                string s;
                foreach ( string p in paragraphs )
                {
                    s = p.Trim();
                    if ( !string.IsNullOrEmpty( s ) )
                        content += s + '\n';
                    /*
                    int k = LengthOfSpaces( p );
                    // This skip the space-only line
                    if ( k != p.Length )
                        content += p.Substring( LengthOfSpaces( p ) ) + '\n';
                    */
                }
                paragraphs = null;

                string ills;
                if ( HasExtractedIllustrations( ref content, out ills ) )
                {
                    Shared.Storage.WriteString( illspath, ills );
                }

                // Write Chapter Content
                Shared.Storage.WriteString( path, Manipulation.PatchSyntax( content ) );
            }
            catch ( Exception ex )
            {
                Logger.Log( ID, ex.Message, LogType.ERROR );
            }
        }

        private bool HasExtractedIllustrations( ref string content, out string ills )
        {
            const string token = "<!--image-->";
            const int tokenl = 12;

            int i = content.LastIndexOf( token );
            int l = 0;
            ills = "";

            // 128 images, big enough for one chapter
            int[][] anchors = new int[ 128 ][];

            if ( i != -1 )
            {
                int j = content.LastIndexOf( token, i );
                while ( j != -1 )
                {
                    // Get Image anchors
                    int[] k = { j, i - j };
                    anchors[ l++ ] = k;
                    i = content.LastIndexOf( token, j );
                    if ( i == -1 ) break;
                    j = content.LastIndexOf( token, i );
                }
                // Replace urls
                string replaced = content.Substring( 0, anchors[ l - 1 ][ 0 ] );
                // Anchors is got in reverse
                for ( i = 0; i < 128; i ++ )
                {
                    ills += content.Substring( anchors[ i ][ 0 ] + tokenl, anchors[ i ][ 1 ] - tokenl ) + "\n";

                    // Append rest of the text contents
                    int a = anchors[ i ][ 0 ] + tokenl + anchors[ i ][ 1 ];
                    if ( anchors[ i + 1 ] == null )
                    {
                        if ( a < content.Length )
                        {
                            replaced += content.Substring( a );
                        }
                        break;
                    }
                    else if ( a < anchors[ i + 1 ][ 0 ] )
                    {
                        replaced += content.Substring( a, anchors[ i + 1 ][ 0 ] - a );
                    }
                }
                // Replaced content
                content = replaced;

                return true;
            }
            return false;
        }
    }
}