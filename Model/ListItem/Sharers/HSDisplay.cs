using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.ListItem.Sharers
{
	sealed class HSDisplay
	{
		public HubScriptItem Item { get; private set; }

		public string Name => Item.Name;

		public HSDisplay( HubScriptItem HSItem )
		{
			Item = HSItem;
		}

		public static string PropertyName( PropertyInfo Info )
		{
			return "";
		}
	}
}