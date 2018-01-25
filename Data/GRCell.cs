using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GR.Data
{
	public class GRCell<T>
	{
		public PropertyInfo Property;
		public int Sorting = 0;

		public Func<T, object> Path = x => x;
		public string Value( T x ) => ( string ) Property.GetValue( Path( x ) );

		public GRCell( PropertyInfo Property )
		{
			this.Property = Property;
		}
	}

}