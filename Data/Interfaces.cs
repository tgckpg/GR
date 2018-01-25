using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

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
		GridLength HSP { get; }

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

		bool ColEnabled( int colIndex );
	}

	public interface IGRRow : IGRRowBase
	{
		IGRTable Table { get; }
	}
}
