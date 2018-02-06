using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

using Net.Astropenguin.Linq;
using GR.Database.Models;
using System.ComponentModel;

namespace GR.Data
{
	public class GRTable<T> : GRRowBase<T>, IGRTable
	{
		public readonly Type GRTableType = typeof( GRTable<T> );

		public double DefaultGL = 200;

		public GridLength H00 { get; set; }
		public GridLength H01 { get; set; }
		public GridLength H02 { get; set; }
		public GridLength H03 { get; set; }
		public GridLength H04 { get; set; }
		public GridLength H05 { get; set; }
		public GridLength H06 { get; set; }
		public GridLength H07 { get; set; }
		public GridLength H08 { get; set; }
		public GridLength H09 { get; set; }

		public GridLength HSP { get; set; } = new GridLength( 0, GridUnitType.Star );

		public int S00 => ColEnabled( 0 ) ? CellProps[ 0 ].Sorting : 0;
		public int S01 => ColEnabled( 1 ) ? CellProps[ 1 ].Sorting : 0;
		public int S02 => ColEnabled( 2 ) ? CellProps[ 2 ].Sorting : 0;
		public int S03 => ColEnabled( 3 ) ? CellProps[ 3 ].Sorting : 0;
		public int S04 => ColEnabled( 4 ) ? CellProps[ 4 ].Sorting : 0;
		public int S05 => ColEnabled( 5 ) ? CellProps[ 5 ].Sorting : 0;
		public int S06 => ColEnabled( 6 ) ? CellProps[ 6 ].Sorting : 0;
		public int S07 => ColEnabled( 7 ) ? CellProps[ 7 ].Sorting : 0;
		public int S08 => ColEnabled( 8 ) ? CellProps[ 8 ].Sorting : 0;
		public int S09 => ColEnabled( 9 ) ? CellProps[ 9 ].Sorting : 0;

		public List<IGRCell> CellProps { get; private set; }
		private Dictionary<IGRCell, int> PropIndexes;

		public GRTable( List<IGRCell> CellProps )
		{
			this.CellProps = CellProps;
			PropIndexes = new Dictionary<IGRCell, int>();

			Headers.ExecEach( x => x.SetValue( this, new GridLength( DefaultGL, GridUnitType.Pixel ) ) );
			CellProps.ExecEach( ( x, i ) => { PropIndexes[ x ] = i; } );
		}

		public IEnumerable<GRRow<T>> _Items;
		public IEnumerable<GRRow<T>> Items
		{
			get => _Items;
			set
			{
				_Items = value;
				NotifyChanged( "Items" );
			}
		}

		private static IReadOnlyList<PropertyInfo> _Headers;
		public IReadOnlyList<PropertyInfo> Headers
			=> _Headers ?? (
				_Headers = Cells.Remap( x => GRTableType.GetProperty( x.Name.Replace( 'C', 'H' ) ) ).ToList().AsReadOnly()
			);

		private static IReadOnlyList<PropertyInfo> _Sortings;
		public IReadOnlyList<PropertyInfo> Sortings
			=> _Sortings ?? (
				_Sortings = Cells.Remap( x => GRTableType.GetProperty( x.Name.Replace( 'C', 'S' ) ) ).ToList().AsReadOnly()
			);

		private string[] _HeaderNames;
		public IReadOnlyList<string> HeaderNames => _HeaderNames ?? ( _HeaderNames = Headers.Remap( x => x.Name ) );
		private string[] _SortingNames;
		public IReadOnlyList<string> SortingNames => _SortingNames ?? ( _SortingNames = Sortings.Remap( x => x.Name ) );

		// Listen for the Column update
		protected override void NotifyCellUpdate( object sender, PropertyChangedEventArgs e )
		{
			int Index = ColIndex( sender.GetType(), e.PropertyName );
			if ( ColEnabled( Index ) )
			{
				NotifyChanged( Cells[ Index ].Name );
			}
		}

		public int ColIndex( Type PropertyOwner, string PropertyName ) => CellProps.FindIndex( x => x.Property.DeclaringType == PropertyOwner && x.Property.Name == PropertyName );

		public bool ColEnabled( int ColIndex )
		{
			return 0 <= ColIndex && ColIndex < Math.Min( Headers.Count, CellProps.Count ) && 0 < ( ( GridLength ) Headers[ ColIndex ].GetValue( this ) ).Value;
		}

		public void SortCol( int ColIndex, int Direction )
		{
			Sortings.ExecEach( ( x, i ) => CellProps[ i ].Sorting = 0 );
			CellProps[ ColIndex ].Sorting = Direction;

			NotifyChanged( SortingNames.ToArray() );
		}

		public void SetCol( int FromCol, int ToCol, bool Enable )
		{
			IEnumerable<PropertyInfo> Cols = Headers;

			if ( 0 < FromCol )
				Cols = Cols.Skip( FromCol );

			if ( FromCol <= ToCol )
				Cols = Cols.Take( ToCol - FromCol + 1 );

			if ( Enable )
			{
				foreach ( PropertyInfo GLInfo in Cols )
				{
					GridLength GL = ( GridLength ) GLInfo.GetValue( this );
					if ( GL.Value == 0 )
					{
						GLInfo.SetValue( this, new GridLength( DefaultGL, GL.GridUnitType ) );
					}
				}
			}
			else
			{
				foreach ( PropertyInfo GLInfo in Cols )
				{
					GridLength GL = ( GridLength ) GLInfo.GetValue( this );
					if ( GL.Value != 0 )
					{
						GLInfo.SetValue( this, new GridLength( 0, GL.GridUnitType ) );
					}
				}
			}

			RefreshCols( FromCol, ToCol );
		}

		public void ResizeCol( int ColIndex, double Delta )
		{
			PropertyInfo Header = Headers[ ColIndex ];
			GridLength GL = ( GridLength ) Header.GetValue( this );

			double k = GL.Value + Delta;
			if ( k < 100 ) k = 100;

			Header.SetValue( this, new GridLength( k, GL.GridUnitType ) );
			NotifyChanged( Header.Name );
		}

		public void Configure( GRTableConfig Config )
		{
			List<IGRCell> OrderedProps = new List<IGRCell>();
			int i = 0;
			foreach ( ColumnConfig Col in Config.Columns )
			{
				int ColIndex = CellProps.FindIndex( x => x.Property.Name == Col.Name );
				if ( ColIndex == -1 ) continue;

				// Set column width
				PropertyInfo GLInfo = Headers[ i ];
				GridLength GL = ( GridLength ) GLInfo.GetValue( this );
				Headers[ i ].SetValue( this, new GridLength( Col.Width, GL.GridUnitType ) );

				// Set column order
				IGRCell Prop = CellProps[ ColIndex ];
				CellProps.RemoveAt( ColIndex );
				OrderedProps.Add( Prop );
				i++;
			}

			OrderedProps.AddRange( CellProps );
			CellProps = OrderedProps;

			RefreshCols( 0, i - 1 );
			SetCol( i, -1, false );
		}

		public bool ToggleCol( IGRCell CellProp )
		{
			int ActiveCols = 0;

			bool CellEnabled = false;

			CellProps.ExecEach( ( x, i ) =>
			{
				bool Enabled = ColEnabled( i );

				if ( Enabled )
					ActiveCols++;

				if( x == CellProp )
				{
					CellEnabled = Enabled;
				}
			} );

			if ( CellEnabled )
			{
				PropIndexes[ CellProp ] = CellProps.IndexOf( CellProp );
				MoveColumn( CellProp, CellProps.Count - 1 );
				ActiveCols--;
			}
			else
			{
				if ( ActiveCols < PropIndexes[ CellProp ] )
				{
					PropIndexes[ CellProp ] = ActiveCols;
				}
				MoveColumn( CellProp, PropIndexes[ CellProp ] );
				ActiveCols++;
			}

			SetCol( PropIndexes[ CellProp ], ActiveCols - 1, true );
			SetCol( ActiveCols, -1, false );

			return !CellEnabled;
		}

		public override void RefreshCols( int FromCol, int ToCol )
		{
			IEnumerable<string> _HdNames = HeaderNames;
			IEnumerable<string> _StNames = SortingNames;

			if ( 0 < FromCol )
			{
				_HdNames = _HdNames.Skip( FromCol );
				_StNames = _StNames.Skip( FromCol );
			}

			if ( FromCol <= ToCol )
			{
				int k = ToCol - FromCol + 1;
				_HdNames = _HdNames.Take( k );
				_StNames = _StNames.Take( k );
			}

			NotifyChanged( _HdNames.Concat( _StNames ).ToArray() );
			base.RefreshCols( FromCol, ToCol );

			Items?.ExecEach( x => x.RefreshCols( FromCol, ToCol ) );
		}

		public void MoveColumn( int FromIndex, int ToIndex )
		{
			if ( FromIndex == ToIndex ) return;

			int ActiveCols = 0;

			CellProps.ExecEach( ( x, i ) =>
			{
				if ( ColEnabled( i ) )
					ActiveCols++;
			} );

			MoveColumn( CellProps[ FromIndex ], ToIndex );
			SetCol( Math.Min( FromIndex, ToIndex ), ActiveCols - 1, true );
			SetCol( ActiveCols, -1, false );
		}

		private void MoveColumn( IGRCell CellProp, int Index )
		{
			int k = CellProps.IndexOf( CellProp );
			int MaxCols = Headers.Count;

			object kH = k < MaxCols ? Headers[ k ].GetValue( this ) : null;

			if ( k < Index )
			{
				for ( int i = k, j = k + 1; i < Index; i++, j++ )
				{
					CellProps[ i ] = CellProps[ j ];
					if ( j < MaxCols )
					{
						Headers[ i ].SetValue( this, Headers[ j ].GetValue( this ) );
					}
				}
			}
			else
			{
				for ( int i = k, j = k - 1; Index < i; i--, j-- )
				{
					CellProps[ i ] = CellProps[ j ];
					if ( i < MaxCols )
					{
						Headers[ i ].SetValue( this, Headers[ j ].GetValue( this ) );
					}
				}
			}

			if ( Index < MaxCols && kH != null )
			{
				Headers[ Index ].SetValue( this, kH );
			}

			CellProps[ Index ] = CellProp;
		}

	}
}