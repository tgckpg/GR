using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

using Net.Astropenguin.Linq;
using GR.Resources;

namespace GR.Data
{
	public class GRTable : GRRowBase
	{
		public static readonly Type GRTableType = typeof( GRTable );

		public GridLength H00 { get; set; } = new GridLength( 100, GridUnitType.Star );
		public GridLength H01 { get; set; } = new GridLength( 100, GridUnitType.Star );
		public GridLength H02 { get; set; } = new GridLength( 100, GridUnitType.Star );
		public GridLength H03 { get; set; } = new GridLength( 100, GridUnitType.Star );
		public GridLength H04 { get; set; } = new GridLength( 100, GridUnitType.Star );
		public GridLength H05 { get; set; } = new GridLength( 100, GridUnitType.Star );
		public GridLength H06 { get; set; } = new GridLength( 100, GridUnitType.Star );
		public GridLength H07 { get; set; } = new GridLength( 100, GridUnitType.Star );
		public GridLength H08 { get; set; } = new GridLength( 100, GridUnitType.Star );
		public GridLength H09 { get; set; } = new GridLength( 100, GridUnitType.Star );

		public GridLength HSP { get; set; } = new GridLength( 0, GridUnitType.Star );

		public static string IconSort( int k ) => k == 0 ? "" : ( k == 1 ? SegoeMDL2.ChevronUp : SegoeMDL2.ChevronDown );
		public int S00 { get; set; }
		public int S01 { get; set; }
		public int S02 { get; set; }
		public int S03 { get; set; }
		public int S04 { get; set; }
		public int S05 { get; set; }
		public int S06 { get; set; }
		public int S07 { get; set; }
		public int S08 { get; set; }
		public int S09 { get; set; }

		public IEnumerable<GRRow> _Items;
		public IEnumerable<GRRow> Items
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

		public override void RefreshCols( int FromCol, int ToCol )
		{
			IEnumerable<string> _HdNames = HeaderNames;

			if ( 0 < FromCol )
				_HdNames = _HdNames.Skip( FromCol );

			if ( FromCol < ToCol )
				_HdNames = _HdNames.Take( ToCol - FromCol + 1 );

			NotifyChanged( _HdNames.ToArray() );
			base.RefreshCols( FromCol, ToCol );

			Items?.ExecEach( x => x.RefreshCols( FromCol, ToCol ) );
		}

		public void SortCol( int ColIndex, int Direction )
		{
			Sortings.ExecEach( x => x.SetValue( this, 0 ) );
			Sortings[ ColIndex ].SetValue( this, Direction );
			NotifyChanged( SortingNames.ToArray() );
		}

		public void SetCol( int FromCol, int ToCol, bool Enable )
		{
			IEnumerable<PropertyInfo> Cols = Headers;

			if ( 0 < FromCol )
				Cols = Cols.Skip( FromCol );

			if ( FromCol < ToCol )
				Cols = Cols.Take( ToCol - FromCol + 1 );

			if ( Enable )
			{
				foreach ( PropertyInfo GLInfo in Cols )
				{
					GridLength GL = ( GridLength ) GLInfo.GetValue( this );
					GLInfo.SetValue( this, new GridLength( 100, GL.GridUnitType ) );
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
	}
}