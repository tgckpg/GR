using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Data
{
	public class GRRow<T> : GRRowBase<T>, IGRRow
	{
		public IGRTable Table { get; set; }

		public GRRow( GRTable<T> Table )
		{
			this.Table = Table;
		}

		protected override string _Cell( int ColIndex )
		{
			return Table.ColEnabled( ColIndex ) ? base._Cell( ColIndex ) : "";
		}
	}
}