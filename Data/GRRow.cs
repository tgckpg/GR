using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Data
{
	public class GRRow : GRRowBase 
	{
		public GRTable Table { get; set; }

		public GRRow( GRTable Table )
		{
			this.Table = Table;
		}

		protected override string _Cell( int ColIndex )
		{
			return Table.ColEnabled( ColIndex ) ? base._Cell( ColIndex ) : "";
		}
	}
}