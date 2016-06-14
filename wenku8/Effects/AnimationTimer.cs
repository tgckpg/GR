using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace wenku8.Effects
{
    class AnimationTimer : DispatcherTimer
    {
        private AnimationTimer() : base() { }

        private static readonly Random R = new Random();
        private static readonly object SyncLock = new object();

        private static AnimationTimer __instance;
        public static AnimationTimer Instance
        {
            get
            {
                if ( __instance == null )
                {
                    lock ( SyncLock )
                    {
                        __instance = new AnimationTimer();
                    }
                }

                return __instance;
            }
        }

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

        public static T RandChoiceFromList<T>( IEnumerable<T> Choices )
        {
            lock( SyncLock )
            {
                return Choices.ElementAt( R.Next( Choices.Count() ) );
            }
        }

        public static T RandChoice<T>( params T[] Choices )
        {
            lock( SyncLock )
            {
                return Choices[ R.Next( Choices.Count() ) ];
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
                return R.Next( minValue, maxValue );
            }
        }

        public static int RandInt( int maxValue )
        {
            lock ( SyncLock )
            {
                return R.Next( maxValue );
            }
        }
    }
}
