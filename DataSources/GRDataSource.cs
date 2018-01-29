using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.DataSources
{
	using Data;

	abstract public class GRDataSource
	{
		abstract public IGRTable Table { get; }

		abstract public void StructTable();
		abstract public void Reload();
		abstract public void Sort( int ColIndex );

		abstract public string ColumnName( IGRCell CellProp );
		abstract public void ItemAction( IGRRow Row );
	}
}