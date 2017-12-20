using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libtranslate;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;

namespace GR.Model
{
	using CompositeElement;
	using ListItem;

	sealed class TradChinese
	{
		private bool IsTrad = false;

		public TradChinese() { IsTrad = Config.Properties.LANGUAGE_TRADITIONAL; }
		public TradChinese( bool IsTrad ) { this.IsTrad = IsTrad; }

		public bool DoTranslate { get { return IsTrad; } }

		private Dictionary<string, bool> PrefList = new Dictionary<string, bool>();

		public string Translate( string v )
		{
			return IsTrad ? Chinese.Traditional( v ) : v;
		}

		public byte[] Translate( byte[] v )
		{
			return IsTrad ? Chinese.Traditional( v ) : v;
		}

		public void SetPrefs( IEnumerable<LocalBook> Books )
		{
			foreach ( LocalBook Book in Books ) PrefList[ Book.aid ] = true;
		}

		public async Task<bool> ConfirmTranslate( string aid, string v )
		{
			if ( !IsTrad ) return false;

			if ( PrefList.ContainsKey( aid ) )
				return PrefList[ aid ];

			bool Confirmed = false;

			StringResources stx = new StringResources( "Message" );

			await Popups.ShowDialog( UIAliases.CreateDialog(
				stx.Str( "TranslateTC" )
				, () => Confirmed = true
				, stx.Str( "Yes" ), stx.Str( "No" )
			) );

			return Confirmed;
		}

	}
}