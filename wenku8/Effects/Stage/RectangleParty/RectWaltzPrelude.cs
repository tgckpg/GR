using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace wenku8.Effects.Stage.RectangleParty
{
    using Config;

    class RectWaltzPrelude : PartyBase
    {
        public static int RectsFactor = 10;

        protected Storyboard Mainboard;

        protected int NumCol = 0;
        protected int NumRow = 0;

        protected double RectWidth = 0;
        protected double RectHeight = 0;

        protected Action Complete { get; set; }

        public Func<Color> RectColor { get; private set; }

        public RectWaltzPrelude( Canvas Stage )
            :base( Stage )
        {
            SortMemberOut();
            SetColor( Properties.APPEARENCE_THEME_MAJOR_COLOR );
        }

        public void SetColor( Color C )
        {
            this.RectColor = () => C;
        }

        public void SetEasing( EasingFunctionBase Easing )
        {
            this.DefaultEasing = Easing;
        }

        public void SetColor( Func<Color> C )
        {
            this.RectColor = C;
        }

        virtual public void SetParty()
        {
            Mainboard = new Storyboard();

            List<int[]> idx = new List<int[]>();
            int i = 0;
            for ( int j = 0; j < NumRow; i++ )
            {
                idx.Add( new int[] { i, j } );

                if ( NumCol <= i )
                {
                    i = -1;
                    j++;
                }
            }

            IOrderedEnumerable<int[]> RandPos = idx.OrderBy( item => NTimer.RandInt() );

            i = 0;

            Stage.Children.Clear();
            foreach ( int[] p in RandPos )
            {
                Rectangle R = NewMember();
                SetAnimation( R, i++, NumCol * NumRow );

                Canvas.SetLeft( R, p[ 0 ] * RectWidth );
                Canvas.SetTop( R, p[ 1 ] * RectHeight );

                Stage.Children.Add( R );
            }
        }

        public void Play()
        {
            if ( Mainboard != null )
            {
                Mainboard.Begin();
                if( Complete != null ) Mainboard.Completed += ( s, e ) => Complete();
            }
        }

        public void Pause()
        {
            if ( Mainboard != null )
            {
                Mainboard.Pause();
            }
        }

        private void SortMemberOut()
        {
            if( Stage.ActualWidth < Stage.ActualHeight )
            {
                NumRow = RectsFactor;
                RectWidth = RectHeight = ( double ) Stage.ActualHeight / ( double ) NumRow;

                NumCol = ( int ) Math.Ceiling( Stage.ActualWidth / RectWidth );
            }
            else
            {
                NumCol = RectsFactor;
                RectWidth = RectHeight = ( double ) Stage.ActualWidth / ( double ) NumCol;

                NumRow = ( int ) Math.Ceiling( Stage.ActualHeight / RectHeight );
            }
        }

        private Rectangle NewMember()
        {
            Rectangle R = new Rectangle();
            R.Width = RectWidth + 2;
            R.Height = RectHeight + 2;

            R.Fill = new SolidColorBrush( RectColor() );

            CompositeTransform CT = new CompositeTransform();

            // Plus 5 for covering edges
            CT.CenterX = 0.5 * RectWidth + 1;
            CT.CenterY = 0.5 * RectHeight + 1;
            R.RenderTransform = CT;

            return R;
        }

        virtual protected void SetAnimation( Rectangle Rect, int i, int l )
        {
            int standfor = ( int ) Math.Round( 500.0 * ( i / ( double ) l ) );

            DoubleAnimationUsingKeyFrames d = CreateKeyFrames( 1, standfor + 200, 500 );
            DoubleAnimationUsingKeyFrames d2 = CreateKeyFrames( 1, standfor + 200, 500 );
            DoubleAnimationUsingKeyFrames d3 = CreateKeyFrames(
                new List<int>() { -360, -270, -180, -90, 0, 90, 180, 270, 360 }
                .OrderBy( x => NTimer.RandInt() ).First()
                , standfor, 700
            );
            Storyboard.SetTarget( d, Rect );
            Storyboard.SetTarget( d2, Rect );
            Storyboard.SetTarget( d3, Rect );
            Storyboard.SetTargetProperty( d, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)" );
            Storyboard.SetTargetProperty( d2, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)" );
            Storyboard.SetTargetProperty( d3, "(UIElement.RenderTransform).(CompositeTransform.Rotation)" );

            Mainboard.Children.Add( d );
            Mainboard.Children.Add( d2 );
            Mainboard.Children.Add( d3 );
        }

        public void OnComplete( Action Complete )
        {
            this.Complete = Complete;
        }

    }
}
