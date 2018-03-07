using System;
using Windows.UI.Xaml.Media.Animation;

namespace GR.Effects
{
	static class Easings
	{
		public static EasingFunctionBase EaseInCubic = new CubicEase() { EasingMode = EasingMode.EaseIn };
		public static EasingFunctionBase EaseOutQuintic = new QuinticEase() { EasingMode = EasingMode.EaseOut };

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