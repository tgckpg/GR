using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.DataModel;

namespace GR.DataSources
{
	using Data;

	abstract public class GRDataSource : ActiveData
	{
		abstract public IGRTable Table { get; }

		/// <summary>
		/// Construct table columns
		/// </summary>
		abstract public void StructTable();

		/// <summary>
		/// Load table configurations for this DataSource
		/// </summary>
		/// <returns></returns>
		abstract public Task ConfigureAsync();
		abstract public Task SaveConfig();

		abstract public void Reload();
		abstract public void Sort( int ColIndex, int Order );
		abstract public void ToggleSort( int ColIndex );

		abstract public string ColumnName( IGRCell CellProp );
		abstract public void ItemAction( IGRRow Row );
	}
}