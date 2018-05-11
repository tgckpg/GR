using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Loaders;

namespace GR.DataSources
{
	using Model.ListItem;

	public class GRHighlights : TreeItem
	{
		virtual public ILoader<ActiveItem> Loader { get; }
		public GRHighlights( string Name ) : base( Name ) { }
	}
}