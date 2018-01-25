using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Linq;

namespace GR.Data
{
	public class GRRowBase<T> : ActiveData, IGRRowBase
	{
		public readonly Type BaseType = typeof( GRRowBase<T> );
		public string C00 => _Cell( 0 );
		public string C01 => _Cell( 1 );
		public string C02 => _Cell( 2 );
		public string C03 => _Cell( 3 );
		public string C04 => _Cell( 4 );
		public string C05 => _Cell( 5 );
		public string C06 => _Cell( 6 );
		public string C07 => _Cell( 7 );
		public string C08 => _Cell( 8 );
		public string C09 => _Cell( 9 );

		public T Source { get; set; }
		public Func<int, object, string> Cell = ( i, x ) => "";

		private static IReadOnlyList<PropertyInfo> _CellProps;
		public IReadOnlyList<PropertyInfo> Cells
		{
			get
			{
				if ( _CellProps == null )
				{
					List<PropertyInfo> _Cells = new List<PropertyInfo>();

					for ( int i = 0; ; i++ )
					{
						PropertyInfo PropInfo = BaseType.GetProperty( string.Format( "C{0:00}", i ) );

						if ( PropInfo == null )
							break;

						_Cells.Add( PropInfo );
					}

					_CellProps = _Cells.AsReadOnly();
				}

				return _CellProps;
			}
		}

		protected string[] _CellNames;
		public IReadOnlyList<string> CellNames => _CellNames ?? ( _CellNames = Cells.Remap( x => x.Name ) );

		virtual protected string _Cell( int ColIndex )
		{
			return Cell( ColIndex, Source );
		}

		virtual public void RefreshCols( int FromCol, int ToCol )
		{
			IEnumerable<string> _Cells = CellNames;

			if ( 0 < FromCol )
				_Cells = _Cells.Skip( FromCol );

			if ( FromCol <= ToCol )
				_Cells = _Cells.Take( ToCol - FromCol + 1 );

			NotifyChanged( _Cells.ToArray() );
		}
	}
}