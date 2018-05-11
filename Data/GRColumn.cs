using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Data
{
	using Resources;
	public class GRColumn
	{
		public static string OrderGlyph( int k ) => k == 0 ? "" : ( k == 1 ? SegoeMDL2.ChevronUp : SegoeMDL2.ChevronDown );
	}
}