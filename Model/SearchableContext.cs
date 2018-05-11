using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.DataModel;

namespace GR.Model
{
	using Interfaces;

	abstract class SearchableContext<T> : ActiveData, ISearchableSection<T>
	{
		protected string Terms;
		protected IEnumerable<T> Data;

		private Func<T, bool> _SQuery;
		public Func<T, bool> SearchQuery
		{
			get
			{
				if ( _SQuery == null )
				{
					if ( typeof( INamable ).GetTypeInfo().IsAssignableFrom( typeof( T ).GetTypeInfo() ) )
					{
						_SQuery = ( T e ) =>
						{
							return ( ( INamable ) e ).Name.IndexOf( SearchTerm, StringComparison.CurrentCultureIgnoreCase ) != -1;
						};
					}
					else
					{
						_SQuery = x => true;
					}
				}
				return _SQuery;
			}
			set
			{
				_SQuery = value;
				NotifyChanged( "SearchSet" );
			}
		}

		public IEnumerable<T> SearchSet
		{
			get => Filter( Data );
			set
			{
				Data = value;
				NotifyChanged( "SearchSet" );
			}
		}

		virtual public string SearchTerm
		{
			get => Terms;
			set
			{
				Terms = value;
				NotifyChanged( "SearchSet" );
			}
		}

		virtual protected IEnumerable<T> Filter( IEnumerable<T> Items )
		{
			if ( Items == null || string.IsNullOrEmpty( SearchTerm ) ) return Items;
			return Items.Where( SearchQuery );
		}
	}
}
