using System;
using System.ComponentModel;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

using Net.Astropenguin.Logging;

namespace wenku8.CompositeElement
{
    [TemplatePart( Name = StageName, Type = typeof( Canvas ) )]
    [TemplatePart( Name = ArcFigureName, Type = typeof( Canvas ) )]
    [TemplatePart( Name = ArcHandName, Type = typeof( Canvas ) )]
    [TemplatePart( Name = HourHandName, Type = typeof( Canvas ) )]
    [TemplatePart( Name = MinuteHandName, Type = typeof( Canvas ) )]
    public class QClock : Control, INotifyPropertyChanged
    {
        public static readonly string ID = typeof( QClock ).Name;

        public static readonly DependencyProperty ArcHandBrushProperty = DependencyProperty.Register( "ArcHandBrush", typeof( Brush ), typeof( QClock ), new PropertyMetadata( new SolidColorBrush( Colors.White ), OnArcHandBrushChanged ) );
        public static readonly DependencyProperty HourHandBrushProperty = DependencyProperty.Register( "HourHandBrush", typeof( Brush ), typeof( QClock ), new PropertyMetadata( new SolidColorBrush( Colors.White ), OnHourHandBrushChanged ) );
        public static readonly DependencyProperty MinuteHandBrushProperty = DependencyProperty.Register( "MinuteHandBrush", typeof( Brush ), typeof( QClock ), new PropertyMetadata( new SolidColorBrush( Colors.White ), OnMinuteHandBrushChanged ) );
        public static readonly DependencyProperty ScalesBrushProperty = DependencyProperty.Register( "ScalesBrush", typeof( Brush ), typeof( QClock ), new PropertyMetadata( new SolidColorBrush( Colors.White ), OnScalesBrushChanged ) );

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register( "Time", typeof( DateTime ), typeof( QClock ), new PropertyMetadata( DateTime.Now, OnTimeChanged ) );
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register( "Progress", typeof( float ), typeof( QClock ), new PropertyMetadata( 0f, OnProgressChanged ) );

        // Angle * 2pi / 360 = Rad
        private const float Deg2Rad = 0.01745329f;

        public Brush ArcHandBrush
        {
            get { return ( Brush ) GetValue( ArcHandBrushProperty ); }
            set { SetValue( ArcHandBrushProperty, value ); }
        }

        public Brush HourHandBrush
        {
            get { return ( Brush ) GetValue( HourHandBrushProperty ); }
            set { SetValue( HourHandBrushProperty, value ); }
        }

        public Brush MinuteHandBrush
        {
            get { return ( Brush ) GetValue( HourHandBrushProperty ); }
            set { SetValue( HourHandBrushProperty, value ); }
        }

        public Brush ScalesBrush
        {
            get { return ( Brush ) GetValue( HourHandBrushProperty ); }
            set { SetValue( HourHandBrushProperty, value ); }
        }

        public DateTime Time
        {
            get { return ( DateTime ) GetValue( TimeProperty ); }
            set
            {
                SetValue( TimeProperty, value );
                SetTime( value );
            }
        }

        public float Progress
        {
            get { return ( float ) GetValue( ProgressProperty ); }
            set
            {
                SetValue( ProgressProperty, value );
                SetProgress( value );
            }
        }

        private const string StageName = "Stage";
        private const string ArcFigureName = "ArcFigure";
        private const string ArcHandName = "ArcHand";
        private const string HourHandName = "HourHand";
        private const string MinuteHandName = "MinuteHand";

        private Canvas Stage;

        private Rectangle HourHand;
        private Rectangle MinuteHand;

        private PathFigure ArcFigure;
        private ArcSegment ArcHand;

        private CompositeTransform HoursRotation;
        private CompositeTransform MinutesRotation;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Property Changed Callbacks
        private void NotifyChanged( string Name )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( Name ) );
        }
        #endregion

        public QClock()
            : base()
        {
            DefaultStyleKey = typeof( QClock );
            Time = DateTime.Now;
        }


        private void SetTime( DateTime Dt )
        {
            if ( HoursRotation == null ) return;

            double SecondS = Dt.Second / 60.0;
            double MinuteS = Dt.Minute / 60.0;

            double HourS = Dt.Hour;
            if ( 12 <= HourS ) HourS -= 12;
            HourS = HourS / 12.0;

            // The rotation of the second hand affects the minute hand
            // by 360 / ( 12 * 5 ) = 6 deg
            MinutesRotation.Rotation = 180.0 + MinuteS * 360 + SecondS * 6;

            // The rotation of the minute hand affects the hour hand
            // by 360 / 12 = 30 deg
            // And the rotation of the second hand affects the hour hand
            // by 1 tick / 60 cycles of second hand
            // i.e. 6 / ( 360 * 60 )
            // which is too small for human eyes
            HoursRotation.Rotation = 180.0 + HourS * 360 + MinuteS * 30;
        }

        private void SetProgress( float p )
        {
            if ( ArcFigure == null ) return;
            Vector2 Center = new Vector2( 0.5f * ( float ) Stage.Width, 0.5f * ( float ) Stage.Height );
            DrawArc( Vector2.Zero, Math.Min( Center.X, Center.Y ) * 0.6f, 0, 360 * p );
        }

        private void DrawArc( Vector2 Center, float Radius, float StartAngle, float EndAngle )
        {
            float S = StartAngle % 360;
            float E = EndAngle % 360;

            if ( E < S )
            {
                ArcHand.SweepDirection = SweepDirection.Counterclockwise;
                ArcHand.IsLargeArc = 180 < S - E;
            }
            else
            {
                ArcHand.SweepDirection = SweepDirection.Clockwise;
                ArcHand.IsLargeArc = 180 < E - S;
            }

            if ( StartAngle < EndAngle && E < S )
            {
                ArcHand.SweepDirection = SweepDirection.Clockwise;
                ArcHand.IsLargeArc = 180 < ( EndAngle - StartAngle ) % 360;
            }

            Vector2 StartPoint = Center + Vector2.Transform(
                new Vector2( 0, -Radius )
                , Matrix3x2.CreateRotation( S * Deg2Rad ) );

            Vector2 EndPoint = Center + Vector2.Transform(
                new Vector2( 0, -Radius )
                , Matrix3x2.CreateRotation( E * Deg2Rad ) );

            ArcFigure.StartPoint = StartPoint.ToPoint();
            ArcHand.Point = EndPoint.ToPoint();
            ArcHand.Size = new Size( Radius, Radius );
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Stage = ( Canvas ) GetTemplateChild( StageName );
            ArcHand = ( ArcSegment ) GetTemplateChild( ArcHandName );
            ArcFigure = ( PathFigure ) GetTemplateChild( ArcFigureName );
            HourHand = ( Rectangle ) GetTemplateChild( HourHandName );
            MinuteHand = ( Rectangle ) GetTemplateChild( MinuteHandName );

            HoursRotation = ( CompositeTransform ) HourHand.RenderTransform;
            MinutesRotation = ( CompositeTransform ) MinuteHand.RenderTransform;

            SetTime( Time );
            SetProgress( Progress );
        }

        private static void OnArcHandBrushChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( QClock ) d ).NotifyChanged( "ArcHandBrush" );
        }

        private static void OnHourHandBrushChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( QClock ) d ).NotifyChanged( "HourHandBrush" );
        }

        private static void OnMinuteHandBrushChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( QClock ) d ).NotifyChanged( "MinuteHandBrush" );
        }

        private static void OnScalesBrushChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( QClock ) d ).NotifyChanged( "ScalesBrush" );
        }

        private static void OnTimeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( QClock ) d ).NotifyChanged( "Time" );
        }

        private static void OnProgressChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( QClock ) d ).NotifyChanged( "Progress" );
        }
    }
}