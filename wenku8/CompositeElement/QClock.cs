using System;
using System.ComponentModel;
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
    [TemplatePart( Name = HourHandName, Type = typeof( Canvas ) )]
    [TemplatePart( Name = MinuteHandName, Type = typeof( Canvas ) )]
    public class QClock : Control, INotifyPropertyChanged
    {
        public static readonly string ID = typeof( QClock ).Name;

        public static readonly DependencyProperty HourHandBrushProperty = DependencyProperty.Register( "HourHandBrush", typeof( Brush ), typeof( QClock ), new PropertyMetadata( new SolidColorBrush( Colors.White ), OnHourHandBrushChanged ) );
        public static readonly DependencyProperty MinuteHandBrushProperty = DependencyProperty.Register( "MinuteHandBrush", typeof( Brush ), typeof( QClock ), new PropertyMetadata( new SolidColorBrush( Colors.White ), OnMinuteHandBrushChanged ) );
        public static readonly DependencyProperty ScalesBrushProperty = DependencyProperty.Register( "ScalesBrush", typeof( Brush ), typeof( QClock ), new PropertyMetadata( new SolidColorBrush( Colors.White ), OnScalesBrushChanged ) );

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register( "Time", typeof( DateTime ), typeof( QClock ), new PropertyMetadata( DateTime.Now, OnTimeChanged ) );

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

        private const string StageName = "Stage";
        private const string HourHandName = "HourHand";
        private const string MinuteHandName = "MinuteHand";

        private Canvas Stage;

        private Rectangle HourHand;
        private Rectangle MinuteHand;

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

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Stage = ( Canvas ) GetTemplateChild( StageName );
            HourHand = ( Rectangle ) GetTemplateChild( HourHandName );
            MinuteHand = ( Rectangle ) GetTemplateChild( MinuteHandName );

            HoursRotation = ( CompositeTransform ) HourHand.RenderTransform;
            MinutesRotation = ( CompositeTransform ) MinuteHand.RenderTransform;

            SetTime( Time );
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
    }
}
