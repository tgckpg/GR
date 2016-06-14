using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace wenku8.Effects.Stage
{
    class PartyBase
    {
        protected Canvas Stage;

        protected EasingFunctionBase DefaultEasing = new CubicEase() { EasingMode = EasingMode.EaseOut };

        public PartyBase( Canvas Stage )
        {
            this.Stage = Stage;
        }

        protected DoubleAnimationUsingKeyFrames CreateKeyFrames( int Value, int Delay, int Time )
        {
            return CreateKeyFrames( Value, Delay, Time, DefaultEasing );
        }

        protected DoubleAnimationUsingKeyFrames CreateKeyFrames( int Value, int Delay, int Time, EasingFunctionBase Easing )
        {
            LinearDoubleKeyFrame still = new LinearDoubleKeyFrame();
            still.Value = 0;
            still.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromMilliseconds( 0 ) );

            EasingDoubleKeyFrame still_still = new EasingDoubleKeyFrame();
            still_still.EasingFunction = Easing;
            still_still.Value = 0;
            still_still.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromMilliseconds( Delay ) );

            EasingDoubleKeyFrame move = new EasingDoubleKeyFrame();

            move.EasingFunction = Easing;
            move.Value = Value;
            move.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromMilliseconds( Delay + Time ) );

            DoubleAnimationUsingKeyFrames d = new DoubleAnimationUsingKeyFrames();
            d.Duration = new Duration( TimeSpan.FromMilliseconds( Delay + Time ) );
            d.KeyFrames.Add( still );
            d.KeyFrames.Add( still_still );
            d.KeyFrames.Add( move );

            return d;
        }

        protected ColorAnimationUsingKeyFrames CreateKeyFrames(
            Color From, Color To
            , int Delay, int Time, int offset = 0
            , ColorAnimationUsingKeyFrames d = null )
        {
            LinearColorKeyFrame still = new LinearColorKeyFrame();
            still.Value = From;
            still.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromMilliseconds( offset ) );

            EasingColorKeyFrame still_still = new EasingColorKeyFrame();
            still_still.EasingFunction = DefaultEasing;
            still_still.Value = From;
            still_still.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromMilliseconds( offset + Delay ) );

            EasingColorKeyFrame move = new EasingColorKeyFrame();

            move.EasingFunction = DefaultEasing;
            move.Value = To;
            move.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromMilliseconds( offset + Delay + Time ) );

            if( d == null ) d = new ColorAnimationUsingKeyFrames();
            d.Duration = new Duration( TimeSpan.FromMilliseconds( offset + Delay + Time ) );
            d.KeyFrames.Add( still );
            d.KeyFrames.Add( still_still );
            d.KeyFrames.Add( move );

            return d;
        }
    }
}
