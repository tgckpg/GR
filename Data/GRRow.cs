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
			Font = DefaultFont;
		}

		private string DefaultCell( int i, object x ) => Table.CellProps[ i ].Value( x );
		private string DefaultFont( int i, object x ) => Table.CellProps[ i ].Font?.Invoke( x ) ?? "Segoe UI";

		protected override string _Cell( int ColIndex ) => Table.ColEnabled( ColIndex ) ? base._Cell( ColIndex ) : "";
		protected override string _Font( int ColIndex ) => Table.ColEnabled( ColIndex ) ? base._Font( ColIndex ) : "Segoe UI";

		protected override void NotifyCellUpdate( object sender, PropertyChangedEventArgs e )
		{
			int Index = Table.ColIndex( sender.GetType(), e.PropertyName );
			if( Table.ColEnabled( Index ) )
			{
				string CellName = Cells[ Index ].Name;
				NotifyChanged( CellName, CellName.Replace( 'C', 'F' ) );
			}
		}

		public void Refresh()
		{
			NotifyChanged( CellNames.Where( ( i, x ) => Table.ColEnabled( i ) ) );
		}
	}
}