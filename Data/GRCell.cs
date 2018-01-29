using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GR.Model.Book;

namespace GR.Data
{
	public class GRCell<T> : IGRCell
	{
		public PropertyInfo Property { get; set; }
		public int Sorting { get; set; } = 0;

		public string Value( object x ) => Value( ( T ) x );

		public Func<T, object> Path = x => x;
		public string Value( T x ) => ( string ) Property.GetValue( Path( x ) );

		public GRCell( PropertyInfo Property )
		{
			this.Property = Property;
		}
	}

}