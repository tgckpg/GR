using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libtranslate;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;

namespace GR.Model.Text
{
	using CompositeElement;
	using Config;
	using ListItem;
	using Loaders;

	sealed class TranslationAPI
	{
		public bool DoTraditional = false;
		public bool DoSyntaxPatch = false;

		private Dictionary<string, bool> PrefList = new Dictionary<string, bool>();

		public Translator Chinese { get; private set; } = new Translator();
		public Translator Custom { get; private set; } = new Translator();
		public Translator VText { get; private set; } = new Translator();

		public TranslationAPI()
		{
			DoTraditional = Properties.LANGUAGE_TRADITIONAL;
			DoSyntaxPatch = GRConfig.System.PatchSyntax;
		}

		public async Task InitContextTranslator()
		{
			TRTable TableLoader = new TRTable();

			if ( DoTraditional )
			{
				Chinese.AddTable( await TableLoader.Get( "ntw_ws2t" ) );
				Chinese.AddTable( await TableLoader.Get( "ntw_ps2t" ) );
			}

			if ( DoSyntaxPatch )
			{
				Custom.AddTable( await TableLoader.Get( "synpatch" ) );
			}
		}

		public async Task InitUITranslators()
		{
			TRTable TableLoader = new TRTable();
			VText.AddTable( await TableLoader.Get( "vertical" ) );
		}

		public void SetPrefs( IEnumerable<LocalBook> Books )
		{
			foreach ( LocalBook Book in Books ) PrefList[ Book.ZItemId ] = true;
		}

		public async Task<bool> ConfirmTranslate( string aid, string v )
		{
			if ( !DoTraditional ) return false;

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