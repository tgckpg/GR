using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace GR.Effects
{
	sealed class NTimer
	{
		private static readonly Random R = new Random();
		private static readonly object SyncLock = new object();

		public static double RandDouble()
		{
			lock ( SyncLock )
			{
				return R.NextDouble();
			}
		}

		public static double RandDouble( double minValue, double maxValue )
		{
			lock ( SyncLock )
			{
				double Factor = R.NextDouble();
				return ( maxValue - minValue ) * Factor + minValue;
			}
		}

		public static double RandDouble( double maxValue )
		{
			lock ( SyncLock )
			{
				return R.NextDouble() * maxValue;
			}
		}

		public static T RandChoice<T>( Random R, params T[] Choices )
		{
			return Choices[ R.Next( Choices.Count() ) ];
		}

		public static T RandChoiceFromList<T>( Random R, IEnumerable<T> Choices )
		{
			return Choices.ElementAt( R.Next( Choices.Count() ) );
		}

		public static T RandChoice<T>( params T[] Choices )
		{
			lock( SyncLock )
			{
				return Choices[ R.Next( Choices.Count() ) ];
			}
		}

		public static T RandChoiceFromList<T>( IEnumerable<T> Choices )
		{
			lock( SyncLock )
			{
				return Choices.ElementAt( R.Next( Choices.Count() ) );
			}
		}

		/// <summary>
		/// Return true with 1/N probability
		/// </summary>
		/// <param name="N">Denumerator</param>
		/// <returns></returns>
		public static bool P( int N )
		{
			lock( SyncLock )
			{
				return R.Next( N ) == 0;
			}
		}

		public static double RandInt()
		{
			lock( SyncLock )
			{
				return R.Next();
			}
		}

		public static int RandInt( int minValue, int maxValue )
		{
			lock ( SyncLock )
			{
				return R.Next( minValue, maxValue + 1 );
			}
		}

		public static int RandInt( int maxValue )
		{
			lock ( SyncLock )
			{
				return R.Next( maxValue );
			}
		}

		public static float RFloat()
		{
			return 2.0f * ( ( float ) R.NextDouble() ) - 1;
		}

		public static float LFloat()
		{
			return ( float ) R.NextDouble();
		}

	}
}