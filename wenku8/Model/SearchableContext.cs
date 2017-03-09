using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.DataModel;

namespace wenku8.Model
{
	using Interfaces;
	using ListItem;

	abstract class SearchableContext : ActiveData, ISearchableSection<ActiveItem>
	{
		protected string Terms;
		protected IEnumerable<ActiveItem> Data;

		public IEnumerable<ActiveItem> SearchSet
		{
			get
			{
				return Filter( Data );
			}

			set
			{
				Data = value;
				NotifyChanged( "SearchSet" );
			}
		}

		virtual public string SearchTerm
		{
			get
			{
				return Terms;
			}
			set
			{
				Terms = value;
				NotifyChanged( "SearchSet" );
			}
		}

		virtual protected IEnumerable<ActiveItem> Filter( IEnumerable<ActiveItem> Items )
		{
			if ( Items == null || string.IsNullOrEmpty( SearchTerm ) ) return Items;

			return Items.Where( ( ActiveItem e ) =>
			 {
				 return e.Name.IndexOf( SearchTerm, StringComparison.CurrentCultureIgnoreCase ) != -1;
			 } );
		}
	}
}
