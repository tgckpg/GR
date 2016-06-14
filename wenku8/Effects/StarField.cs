using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace wenku8.Effects
{
    using Model.Interfaces;

    class StarField
    {
        public static int NumStars = 50;
        private DispatcherTimer GalaticState;

        private Canvas Stage;
        private string ParallaxTarget;

        private bool Drawn = false;
        public StarField( Canvas Stage, string ParallaxTarget )
        {
            this.ParallaxTarget = ParallaxTarget;
            this.Stage = Stage;
            GalaticState = AnimationTimer.Instance;
            GalaticState.Interval = TimeSpan.FromMilliseconds( 10 );
        }

        public void Stop()
        {
            GalaticState.Stop();
        }

        public void Draw()
        {
            if( Drawn )
            {
                GalaticState.Start();
                return;
            }

            Drawn = true;

            double W = Resources.LayoutSettings.ScreenWidth * 2;
            SolidColorBrush B = new SolidColorBrush( Config.Properties.APPEARENCE_THEME_MAJOR_COLOR );

            for ( int i = 0; i < NumStars; i++ )
            {
                Stage.Children.Add( NewStar( W, B ) );
            }

            GalaticState.Start();
        }

        public void AssignRoam( double x, double y, IRoamable Roamer )
        {
            double i = 0;
            double j = AnimationTimer.RandDouble() * 0.01;
            double k = AnimationTimer.RandDouble() * 100;
            double i2 = 0;
            double j2 = AnimationTimer.RandDouble() * 0.01;
            double k2 = AnimationTimer.RandDouble() * 100;

            double oy = 0;
            double ox = 0;

            GalaticState.Tick += ( sender, e ) =>
            {
                if ( !Roamer.CanRoam ) return;
                oy = y + k * Math.Sin( i += j );
                ox = x + k2 * Math.Sin( i2 += j2 );

                Roamer.Roam( ox, oy );
            };
        }

        private class RoamingEllipse : IRoamable
        {
            public bool CanRoam { get { return true; } }
            private Ellipse o;

            public RoamingEllipse( Ellipse ell )
            {
                o = ell;
            }

            public void Roam( double ox, double oy )
            {
                Canvas.SetLeft( o, ox );
                Canvas.SetTop( o, oy );
            }
        }

        private Ellipse NewStar( double W, Brush B )
        {
            Ellipse o = new Ellipse();

            double sh = Resources.LayoutSettings.ScreenHeight;

            double x = AnimationTimer.RandDouble() * 2 * W;
            double y = AnimationTimer.RandDouble();
            y = sh * ( 1 - y * y );

            double dia = 40 + ( AnimationTimer.RandDouble() * 100 * ( y / sh ) );

            double s = AnimationTimer.RandDouble();

            Canvas.SetLeft( o, x );
            Canvas.SetTop( o, y );

            AssignRoam( x, y, new RoamingEllipse( o ) );

            Windows.UI.Xaml.Data.Binding bind = new Windows.UI.Xaml.Data.Binding();
            bind.ConverterParameter = -s;
            bind.ElementName = ParallaxTarget;
            bind.Converter = new Converters.ParallaxConverter();
            bind.Path = new PropertyPath( "HorizontalOffset" );

            o.Fill = B;

            o.RenderTransform = new CompositeTransform();

            BindingOperations.SetBinding(
                o.RenderTransform
                , CompositeTransform.TranslateXProperty
                , bind
            );

            o.Width = o.Height = dia;

            return o;
        }
    }
}
