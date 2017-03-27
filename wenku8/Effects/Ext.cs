using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Effects
{
	static class Ext
	{
		public static T Clamp<T>( this T Val, T Min, T Max ) where T : IComparable<T>
		{
			if ( Val.CompareTo( Min ) < 0 ) return Min;
			else if ( 0 < Val.CompareTo( Max ) ) return Max;

			return Val;
		}
	}
}