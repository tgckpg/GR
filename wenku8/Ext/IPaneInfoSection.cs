using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.DataModel;

namespace wenku8.Ext
{
	interface IPaneInfoSection
	{
		event PropertyChangedEventHandler PropertyChanged;
		IEnumerable<ActiveData> Data { get; set; }
	}
}