using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace wenku8.Effects
{
    static class SimpleStory
    {
        public static void DoubleAnimation( Storyboard Board, UIElement Element, string Property, double From, double To, double Duration = 350 )
        {
            DoubleAnimationUsingKeyFrames d = new DoubleAnimationUsingKeyFrames();

            EasingDoubleKeyFrame still = new EasingDoubleKeyFrame();
            still.Value = From;
            still.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromSeconds( 0 ) );
            still.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            EasingDoubleKeyFrame move = new EasingDoubleKeyFrame();
            move.Value = To;
            move.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            move.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromMilliseconds( Duration ) );

            d.Duration = new Duration( TimeSpan.FromMilliseconds( Duration ) );

            d.KeyFrames.Add( still );
            d.KeyFrames.Add( move );

            Storyboard.SetTarget( d, Element );
            Storyboard.SetTargetProperty( d, Property );
            Board.Children.Add( d );
        }
    }
}