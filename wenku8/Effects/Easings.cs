using System;

namespace wenku8.Effects
{
	static class Easings
	{
		public static float OutQuintic( float t, float d )
		{
			if ( d <= t ) return 1;
			return ( ( t = t / d - 1 ) * t * t * t * t + 1 );
		}

		public static float InOutQuintic( float t, float d )
		{
			if ( ( t /= d / 2 ) < 1 )
				return .5f * t * t * t * t * t;
			else
				return .5f * ( ( t -= 2 ) * t * t * t * t + 2 );
		}
	}
}