using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace GR.GSystem
{
	using Model.ListItem;

	sealed class BackStackManager : List<NameValue<Page>>
	{
		public bool CanGoBack
		{
			get { return this.FirstOrDefault() != null; }
		}

		public NameValue<Page> Back()
		{
			NameValue<Page> P = this[ Count - 1 ];
			RemoveAt( Count - 1 );

			return P;
		}

		public void Add( string Name, Page Incoming )
		{
			NameValue<Page> OP = this.FirstOrDefault( x => x.Name == Name );
			if ( OP != null )
			{
				Add( OP );
				return;
			}

			Add( new NameValue<Page>( Name, Incoming ) );
		}

		public Page Get( string Name )
		{
			return this.FirstOrDefault( x => x.Name == Name )?.Value;
		}

		new public void Clear()
		{
			ForEach( x => ( x.Value as IDisposable )?.Dispose() );
			base.Clear();
		}

		public void Remove( string Name )
		{
			RemoveAll( ( x ) =>
			{
				if ( x.Name == Name )
				{
					( x.Value as IDisposable )?.Dispose();
					return true;
				}
				return false;
			} );
		}

	}
}