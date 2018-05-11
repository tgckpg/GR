using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;

namespace GR.Effects
{
	static class Ext
	{
		public static T Clamp<T>( this T Val, T Min, T Max ) where T : IComparable<T>
		{
			if ( Val.CompareTo( Min ) < 0 ) return Min;
			else if ( 0 < Val.CompareTo( Max ) ) return Max;

			return Val;
		}

		public static bool IsZero( this Size S )
		{
			return S.IsEmpty || S.Width == 0 || S.Height == 0;
		}
	}
}