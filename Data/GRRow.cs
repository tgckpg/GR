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
			Cell = DefaultCell;
		}

		private string DefaultCell( int i, object x )
		{
			return Table.CellProps[ i ].Value( x );
		}

		protected override string _Cell( int ColIndex )
		{
			return Table.ColEnabled( ColIndex ) ? base._Cell( ColIndex ) : "";
		}

		protected override void NotifyCellUpdate( object sender, PropertyChangedEventArgs e )
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
	}
}