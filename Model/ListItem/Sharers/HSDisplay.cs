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

		private static StringResources _stx;
		private static StringResources stx => _stx ?? ( _stx = StringResources.Load( "AppResources", "Book" ) );

		public HSDisplay( HubScriptItem HSItem )
		{
			Item = HSItem;
		}

		public static string PropertyName( PropertyInfo Info )
		{
			switch ( Info.Name )
			{
				case "Author":
					return stx.Text( "HS_Maintainer" );
				case "Name":
				case "Description":
					return stx.Text( Info.Name );
				case "Zone":
					return stx.Text( "HS_Zone" );
				case "Status":
					return stx.Text( "Status", "Book" );
			}

			return Info.Name;
		}
	}
}