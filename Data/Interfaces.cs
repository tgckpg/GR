using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

using GR.Database.Models;

namespace GR.Data
{
	public interface IGRRowBase : INotifyPropertyChanged
	{
		string C00 { get; }
		string C01 { get; }
		string C02 { get; }
		string C03 { get; }
		string C04 { get; }
		string C05 { get; }
		string C06 { get; }
		string C07 { get; }
		string C08 { get; }
		string C09 { get; }

		string F00 { get; }
		string F01 { get; }
		string F02 { get; }
		string F03 { get; }
		string F04 { get; }
		string F05 { get; }
		string F06 { get; }
		string F07 { get; }
		string F08 { get; }
		string F09 { get; }

		object CellData { get; }
	}

	public interface IGRTable : IGRRowBase
	{
		GridLength H00 { get; }
		GridLength H01 { get; }
		GridLength H02 { get; }
		GridLength H03 { get; }
		GridLength H04 { get; }
		GridLength H05 { get; }
		GridLength H06 { get; }
		GridLength H07 { get; }
		GridLength H08 { get; }
		GridLength H09 { get; }

		int S00 { get; }
		int S01 { get; }
		int S02 { get; }
		int S03 { get; }
		int S04 { get; }
		int S05 { get; }
		int S06 { get; }
		int S07 { get; }
		int S08 { get; }
		int S09 { get; }

		List<IGRCell> CellProps { get; }
		IReadOnlyList<PropertyInfo> Headers { get; }
		IReadOnlyList<PropertyInfo> Sortings { get; }

		void Configure( GRTableConfig config );

		bool ColEnabled( int ColIndex );
		bool ToggleCol( IGRCell Cell );
		int ColIndex( Type PropertyOwner, string PropertyName );

		void SetCol( int FromCol, int ToCol, bool Enable );
		bool ResizeCol( int ColIndex, double x );
		void MoveColumn( int FromCol, int ToCol );
	}

	public interface IGRRow : IGRRowBase
	{
		IGRTable Table { get; }
		void Refresh();
	}

	public interface IGRCell
	{
		int Sorting { get; set; }
		PropertyInfo Property { get; }

		Func<object, string> Font { get; }
		string Value( object x );
	}

}