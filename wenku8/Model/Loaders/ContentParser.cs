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
    using Settings;
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

            ills = "";

            int i = content.IndexOf( token );

            if ( i == -1 ) return false;

            int nIllus = 0;
            string Replaced = 0 < i ? content.Substring( 0, i ) : "";
            int j = content.IndexOf( token, i + tokenl );

            while ( !( i == -1 || j == -1 ) )
            {
                ills += content.Substring( i + tokenl, j - i - tokenl ) + "\n";
                i = content.IndexOf( token, j + tokenl );

                string ImgFlag = "\n" + AppKeys.ANO_IMG + nIllus.ToString() + "\n";

                if ( i == -1 )
                {
                    Replaced += ImgFlag + content.Substring( j + tokenl );
                    break;
                }
                else
                {
                    Replaced += ImgFlag + content.Substring( j + tokenl, i - j - tokenl );
                }

                j = content.IndexOf( token, i + tokenl );

                nIllus++;
            }

            content = Replaced;

            return !string.IsNullOrEmpty( ills );
        }
    }
}