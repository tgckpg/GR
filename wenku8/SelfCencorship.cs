using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Popups;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;

namespace wenku8
{
    using System.Messages;

    class SelfCencorship
    {
        private string[] CencoredPhases;
        public SelfCencorship()
        {
            TextReader Text = File.OpenText( "Strings/badwords.txt" );

            XDocument xml = XDocument.Parse( Text.ReadToEnd() );

            IEnumerable<XElement> k = xml.Descendants( "item" );
            int l;
            CencoredPhases = new string[ l = k.Count() ];
            for ( int i = 0; i < l; i++ )
            {
                CencoredPhases[ i ] = k.ElementAt( i ).Value;
            }
        }

        public async Task<bool> Passed( string Text )
        {
            if ( string.IsNullOrEmpty( Text ) ) return true;
            string Alert = CencoredPhases.FirstOrDefault( ( x ) => Text.Contains( x ) );

            if ( Alert == null ) return true;

            StringResources stx = new StringResources( "Message" );
            MessageDialog Msg = new MessageDialog( stx.Str( "Cencorship" ) );
            await Popups.ShowDialog( Msg );

            return false;
        }

    }
}
