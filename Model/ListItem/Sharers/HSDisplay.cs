using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Loaders;

namespace GR.Model.ListItem.Sharers
{
	sealed class HSDisplay
	{
		public HubScriptItem Item { get; private set; }

		public string Name => Item.Name;
		public string Description => Item.Desc;
		public string Author => Item.Author;
		public string Zone => string.Join( ", ", Item.Zone );
		public string Status => Item.Public ? stx.Text( "Public" ) : stx.Text( "Private" );

		private static StringResBg _stx;
		private StringResBg stx => _stx ?? ( _stx = new StringResBg( "AppResources" ) );

		public HSDisplay( HubScriptItem HSItem )
		{
			Item = HSItem;
		}

		public static string PropertyName( PropertyInfo Info )
		{
			return Info.Name;
		}
	}
}