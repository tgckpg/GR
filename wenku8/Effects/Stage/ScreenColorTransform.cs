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
using Windows.UI.Xaml.Shapes;

namespace wenku8.Effects.Stage
{
    class ScreenColorTransform
    {
        private Canvas Stage;

        private Action Complete { get; set; }
        public bool DisposeOnComplete = false;

        private Storyboard sb;

        public ScreenColorTransform( Canvas Stage )
        {
            this.Stage = Stage;
        }

        public void SetScreen( Color From, Color To )
        {
            Rectangle Rect = new Rectangle();
            SolidColorBrush Brush = new SolidColorBrush( From );
            Rect.Fill = Brush;
            Rect.Width = Stage.ActualWidth;
            Rect.Height = Stage.ActualHeight;

            sb = new Storyboard();
            ColorAnimation CAnime = new ColorAnimation();
            CAnime.Duration = TimeSpan.FromMilliseconds( 500 );
            CAnime.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            CAnime.From = From;
            CAnime.To = To;

            Storyboard.SetTarget( CAnime, Brush );
            Storyboard.SetTargetProperty( CAnime, "Color" );
            sb.Children.Add( CAnime );

            Stage.Children.Clear();
            Stage.Children.Add( Rect );
        }

        public void Play()
        {
            sb.Begin();
            sb.Completed += Sb_Completed;
        }

        private void Sb_Completed( object sender, object e )
        {
            sb.Completed -= Sb_Completed;

            if( DisposeOnComplete )
            {
                Stage.Children.Clear();
            }

            if ( Complete != null ) Complete();
        }

        internal void OnComplete( Action Complete )
        {
            this.Complete = Complete;
        }
    }
}
