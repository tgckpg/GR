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
        public static void DoubleAnimation( Storyboard Board, DependencyObject Element, string Property, double From, double To, double Duration = 350, double Delay = 0 )
        {
            DoubleAnimationUsingKeyFrames d = new DoubleAnimationUsingKeyFrames();

            EasingDoubleKeyFrame still = new EasingDoubleKeyFrame();
            still.Value = From;
            still.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromSeconds( 0 ) );
            still.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            if ( 0 < Delay )
            {
                LinearDoubleKeyFrame still_still = new LinearDoubleKeyFrame();
                still_still.Value = From;

                still_still.KeyTime = still.KeyTime;
                still.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromMilliseconds( Delay ) );

                d.KeyFrames.Add( still_still );
            }

            EasingDoubleKeyFrame move = new EasingDoubleKeyFrame();
            move.Value = To;
            move.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            move.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromMilliseconds( Delay + Duration ) );

            d.Duration = new Duration( TimeSpan.FromMilliseconds( Delay + Duration ) );

            d.KeyFrames.Add( still );
            d.KeyFrames.Add( move );

            Storyboard.SetTarget( d, Element );
            Storyboard.SetTargetProperty( d, Property );
            Board.Children.Add( d );
        }

        public static void ObjectAnimation( Storyboard Board, DependencyObject Element, string Property, object From, object To, double Duration = 350, double Delay = 0 )
        {
            ObjectAnimationUsingKeyFrames d = new ObjectAnimationUsingKeyFrames();

            DiscreteObjectKeyFrame still = new DiscreteObjectKeyFrame();
            still.Value = From;
            still.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromSeconds( 0 ) );

            if ( 0 < Delay )
            {
                DiscreteObjectKeyFrame still_still = new DiscreteObjectKeyFrame();
                still_still.Value = To;
                still_still.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromMilliseconds( Delay ) );

                d.KeyFrames.Add( still_still );
            }

            DiscreteObjectKeyFrame move = new DiscreteObjectKeyFrame();
            move.Value = To;
            move.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromMilliseconds( Delay + Duration ) );

            d.Duration = new Duration( TimeSpan.FromMilliseconds( Delay + Duration ) );

            d.KeyFrames.Add( still );
            d.KeyFrames.Add( move );

            Storyboard.SetTarget( d, Element );
            Storyboard.SetTargetProperty( d, Property );
            Board.Children.Add( d );
        }
    }
}