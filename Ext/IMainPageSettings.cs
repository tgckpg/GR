using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR.Model.ListItem;

namespace GR.Ext
{
	interface IMainPageSettings
	{
		IEnumerable<SubtleUpdateItem> NavSections();
		ActiveItem SelectedSection { get; }

		Tuple<Type, string> PayloadCommand( string payload );
	}
}
