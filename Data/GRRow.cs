using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;

using Net.Astropenguin.Linq;

namespace GR.Data
{
	public class GRRow<T> : GRRowBase<T>, IGRRow
	{
		public IGRTable Table { get; set; }

		public GRRow( GRTable<T> Table )
		{
			this.Table = Table;
		}

		protected override string _Cell( int ColIndex )
		{
			return Table.ColEnabled( ColIndex ) ? base._Cell( ColIndex ) : "";
		}

		protected override void NotifyColUpdate( object sender, PropertyChangedEventArgs e )
		{
			int Index = Table.ColIndex( sender.GetType(), e.PropertyName );
			if( Table.ColEnabled( Index ) )
			{
				NotifyChanged( Cells[ Index ].Name );
			}
		}

		public void Refresh()
		{
			NotifyChanged( CellNames.Where( ( i, x ) => Table.ColEnabled( i ) ) );
		}

		private static Func<object, FlyoutBase> NullBase = x => null;

		private Func<object, FlyoutBase> _ContextMenu;
		public Func<object, FlyoutBase> ContextMenu
		{
			get => _ContextMenu ?? NullBase;
			set => _ContextMenu = value;
		}
	}
}