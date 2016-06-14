using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wenku8.Settings.Theme;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace wenku8.Effects.Stage.RectangleParty
{
    class RectWaltzInterlude : RectWaltzPrelude 
    {
        public RectWaltzInterlude( Canvas Stage )
            :base( Stage )
        {
            SetEasing( null );
        }

        public override void SetParty()
        {
            base.SetParty();
            Mainboard.RepeatBehavior = RepeatBehavior.Forever;
        }

        protected override void SetAnimation( Rectangle Rect, int i, int l )
        {
            int standfor = i * 1000 + AnimationTimer.RandInt( 5000 );

            DoubleAnimationUsingKeyFrames d1 = CreateKeyFrames(
                90, standfor, 200
            );

            Storyboard.SetTarget( d1, Rect );
            Storyboard.SetTargetProperty( d1, "(UIElement.RenderTransform).(CompositeTransform.Rotation)" );

            ColorItem CItem = new ColorItem( "A", RectColor() );
            Color C1 = CItem.TColor;
            CItem.L = AnimationTimer.RandInt( 0, 100 );
            Color C2 = CItem.TColor;

            ColorAnimationUsingKeyFrames d2 = CreateKeyFrames( C1, C2, standfor, 200 );

            CreateKeyFrames( C2, C1, 1000 + AnimationTimer.RandInt( 1000 ), AnimationTimer.RandInt( 200, 5000 ), standfor + 200, d2 );

            SolidColorBrush B = new SolidColorBrush();
            Rect.Fill = B;

            Storyboard.SetTarget( d2, B );
            Storyboard.SetTargetProperty( d2, "Color" );

            Mainboard.Children.Add( d1 );
            Mainboard.Children.Add( d2 );
        }

    }
}
