using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

using Net.Astropenguin.Linq;

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

		public int S00 => CellProps[ 0 ].Sorting;
		public int S01 => CellProps[ 1 ].Sorting;
		public int S02 => CellProps[ 2 ].Sorting;
		public int S03 => CellProps[ 3 ].Sorting;
		public int S04 => CellProps[ 4 ].Sorting;
		public int S05 => CellProps[ 5 ].Sorting;
		public int S06 => CellProps[ 6 ].Sorting;
		public int S07 => CellProps[ 7 ].Sorting;
		public int S08 => CellProps[ 8 ].Sorting;
		public int S09 => CellProps[ 9 ].Sorting;

		public List<GRCell<T>> CellProps { get; private set; }
		private Dictionary<GRCell<T>, int> PropIndexes;

		public GRTable( List<GRCell<T>> CellProps )
		{
			this.CellProps = CellProps;
			PropIndexes = new Dictionary<GRCell<T>, int>();

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

		public bool ColEnabled( int ColIndex )
		{
			return ColIndex < Headers.Count && 0 < ( ( GridLength ) Headers[ ColIndex ].GetValue( this ) ).Value;
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
					GLInfo.SetValue( this, new GridLength( DefaultGL, GL.GridUnitType ) );
				}
			}
			else
			{
				foreach ( PropertyInfo GLInfo in Cols )
				{
					GridLength GL = ( GridLength ) GLInfo.GetValue( this );
					GLInfo.SetValue( this, new GridLength( 0, GL.GridUnitType ) );
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

		public bool ToggleCol( GRCell<T> CellProp )
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

		private void MoveColumn( GRCell<T> CellProp, int Index )
		{
			int k = CellProps.IndexOf( CellProp );

			if ( k < Index )
			{
				for ( int i = k; i < Index; i++ )
				{
					CellProps[ i ] = CellProps[ i + 1 ];
				}
			}
			else
			{
				for ( int i = k; Index < i; i-- )
				{
					CellProps[ i ] = CellProps[ i - 1 ];
				}
			}

			CellProps[ Index ] = CellProp;
		}

	}
}