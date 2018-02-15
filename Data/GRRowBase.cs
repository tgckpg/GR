using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Linq;

namespace GR.Data
{
	abstract public class GRRowBase<T> : ActiveData, IGRRowBase
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

		public string F00 => _Font( 0 );
		public string F01 => _Font( 1 );
		public string F02 => _Font( 2 );
		public string F03 => _Font( 3 );
		public string F04 => _Font( 4 );
		public string F05 => _Font( 5 );
		public string F06 => _Font( 6 );
		public string F07 => _Font( 7 );
		public string F08 => _Font( 8 );
		public string F09 => _Font( 9 );

		public object CellData => _Source;

		private T _Source;
		public T Source
		{
			get => _Source;
			set
			{
				if( _Source is INotifyPropertyChanged _OSrc )
				{
					_OSrc.PropertyChanged -= NotifyCellUpdate;
				}

				_Source = value;

				if( _Source is INotifyPropertyChanged _NSrc )
				{
					_NSrc.PropertyChanged += NotifyCellUpdate;
				}
			}
		}

		abstract protected void NotifyCellUpdate( object sender, PropertyChangedEventArgs e );

		public Func<int, object, string> Cell = ( i, x ) => "";
		public Func<int, object, string> Font = ( i, x ) => "Segoe UI";

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

		virtual protected string _Font( int ColIndex )
		{
			return Font( ColIndex, Source );
		}

		virtual public void RefreshCols( int FromCol, int ToCol )
		{
			IEnumerable<string> _Cells = CellNames;

			if ( 0 < FromCol )
				_Cells = _Cells.Skip( FromCol );

			if ( FromCol <= ToCol )
				_Cells = _Cells.Take( ToCol - FromCol + 1 );

			NotifyChanged( _Cells.Concat( _Cells.Remap( x => x.Replace( 'C', 'F' ) ) ).ToArray() );
		}
	}
}