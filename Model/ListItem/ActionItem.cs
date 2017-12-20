using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.ListItem
{
	sealed class ActionItem : ActiveItem
	{
		public object Param { get; private set; }

		public ActionItem( string Name, string Desc, object ActionParam )
			:base( Name, Desc, null )
		{
			Param = ActionParam;
		}
	}
}
