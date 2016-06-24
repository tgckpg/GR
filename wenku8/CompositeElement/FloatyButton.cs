using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

using Net.Astropenguin.Logging;

namespace wenku8.CompositeElement
{
    using Effects;
    using Model.Interfaces;

    enum FloatyState
    {
        NONE = 0,
        NORMAL = 1,
        MOUSE_OVER = 2,
        MOUSE_DOWN = 4,
        EXPLODE = 8,
        VANQUISH = 16,
    }

    public static class ListExt
    {
        public static void ForEach<T>( this IEnumerable<T> TList, Action<T, T> CurrNext )
        {
            IEnumerator<T> Enu = TList.GetEnumerator();
            T Next = default( T );
            while( Next != null || Enu.MoveNext() )
            {
                T Current = Enu.Current;
                Next = Enu.MoveNext() ? Enu.Current : default( T );

                CurrNext( Current, Next );
            }
        }
    }

    [TemplatePart( Name = RingsName, Type = typeof( Canvas ) )]
    [TemplatePart( Name = RingTextName, Type = typeof( Canvas ) )]
    public sealed class FloatyButton : Control, IStar
    {
        public static readonly string ID = typeof( FloatyButton ).Name;

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register( "Title", typeof( string ), typeof( FloatyButton ), new PropertyMetadata( "{Title}", OnTitleChanged ) );
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register( "ImageSource", typeof( ImageSource ), typeof( FloatyButton ), new PropertyMetadata( null, OnImageSourceChanged ) );

        public static readonly DependencyProperty OuterRingBrushProperty = DependencyProperty.Register( "OuterRingBrush", typeof( double ), typeof( FloatyButton ), new PropertyMetadata( new SolidColorBrush( Colors.Black ), OnOuterRingChanged ) );
        public static readonly DependencyProperty TextBrushProperty = DependencyProperty.Register( "TextBrush", typeof( double ), typeof( FloatyButton ), new PropertyMetadata( new SolidColorBrush( Colors.Black ), OnSimpleTextChanged ) );

        public static readonly DependencyProperty TextRotationProperty = DependencyProperty.Register( "TextRotation", typeof( double ), typeof( FloatyButton ), new PropertyMetadata( 0.0, OnSimpleTextChanged ) );
        public static readonly DependencyProperty TextSpeedProperty = DependencyProperty.Register( "TextSpeed", typeof( double ), typeof( FloatyButton ), new PropertyMetadata( 0.15, OnSimpleTextChanged ) );
        public static readonly DependencyProperty IrisFactorProperty = DependencyProperty.Register( "IrisFactor", typeof( double ), typeof( FloatyButton ), new PropertyMetadata( 0.15, OnIrisFactorChanged ) );

        public string Title
        {
            get { return ( string ) GetValue( TitleProperty ); }
            set { SetValue( TitleProperty, value ); }
        }

        public ImageSource ImageSource
        {
            get { return ( ImageSource ) GetValue( ImageSourceProperty ); }
            set { SetValue( ImageSourceProperty, value ); }
        }

        public Brush TextBrush
        {
            get { return ( Brush ) GetValue( TextBrushProperty ); }
            set { SetValue( TextBrushProperty, value ); }
        }

        public Brush OuterRingBrush
        {
            get { return ( Brush ) GetValue( OuterRingBrushProperty ); }
            set { SetValue( OuterRingBrushProperty, value ); }
        }

        public double IrisFactor
        {
            get { return ( double ) GetValue( IrisFactorProperty ); }
            set { SetValue( IrisFactorProperty, value ); }
        }

        public double TextRotation
        {
            get { return ( double ) GetValue( TextRotationProperty ); }
            set { SetValue( TextRotationProperty, value ); }
        }

        public double TextSpeed
        {
            get { return ( double ) GetValue( TextSpeedProperty ); }
            set { SetValue( TextSpeedProperty, value ); }
        }

        private const string RingsName = "Rings";
        private const string RingTextName = "RingText";

        private Canvas Rings;
        private Canvas RingText;

        private Ellipse OuterRing;
        private Ellipse ImageRing;

        private FloatyState _state = FloatyState.NORMAL;
        private FloatyState State
        {
            get { return _state; }
            set
            {
                // State is locked if the current state does no allow changes
                if ( ( _state & ( FloatyState.EXPLODE | FloatyState.VANQUISH ) ) != 0 )
                    return;
                _state = value;
                UpdateState();
            }
        }

        private bool Stop = true;
        private bool Binded = false;
        public bool CanRoam { get; private set; }

        public bool Reacting
        {
            get
            {
                return ( State & ( FloatyState.EXPLODE | FloatyState.MOUSE_OVER | FloatyState.MOUSE_DOWN ) ) != 0;
            }
        }

        public FloatyButton()
        {
            DefaultStyleKey = typeof( FloatyButton );
        }

        private Storyboard RingRotateStory = new Storyboard();

        public void BindTimer( DispatcherTimer DTimer )
        {
            if ( Binded ) return;

            EventHandler<object> evt = ( s, e ) =>
            {
                if ( Stop ) return;
                StateChange();
            };

            DTimer.Tick += evt;

            Binded = true;
        }

        private bool InVisualUpdate = false;
        private ImageBrush ImageBrushy = new ImageBrush();

        private double CanvasDia = 0;

        private double RingOrigin = 0;
        private double ImageOrigin = 0;
        private double OuterRingDia = 0;
        private double ImageRingDia = 0;

        private double TowardsRingDia = 0;
        private double TowardsImageDia = 0;

        private double TotalTitleWidth = 0;
        private double MaxTitleHeight = 0;

        private const double BlossomFactor = 3;
        // This should never be 0
        private double BlossomDia = 1;


        // Title Rotation Offsets
        private double _rRDelta = 0;

        private TaskCompletionSource<bool> Stopped;

        // Run on every tick, codes here should be optimized
        // and as light-weight as possible
        private void StateChange()
        {
            double Dia = ImageRing.Width;
            ParametricCubic( ref Dia, TowardsImageDia, 0.75, 0.25 );
            ImageRing.Width = ImageRing.Height = Dia;
            ImageOrigin = 0.5 * ( CanvasDia - Dia );

            bool Blossom = ( State & ( FloatyState.EXPLODE | FloatyState.VANQUISH ) ) != 0;
            if ( Blossom )
            {
                Dia = BlossomFactor * ImageRing.StrokeThickness;

                if ( Dia < 0.10 )
                {
                    Deactivate();
                    return;
                }

                ParametricCubic( ref Dia, 0 );
                double DiaRatio = BlossomFactor * Dia / BlossomDia;
                OuterRing.Opacity = DiaRatio;
                VisTitle.ForEach( C => C.Opacity = DiaRatio );
            }
            else if ( Math.Abs( TowardsImageDia - Dia ) < 0.10 )
            {
                // On Growth End
                StopGrowth();

                // Do not intercept VisualUpdate
                if ( !InVisualUpdate ) return;
            }

            ImageRing.StrokeThickness = 0.5 * Dia;

            Dia = OuterRing.Width;
            ParametricCubic( ref Dia, TowardsRingDia, 0.75, 0.25 );
            OuterRing.Width = OuterRing.Height = Dia;
            RingOrigin = 0.5 * ( CanvasDia - Dia );

            // The target binding ring diameter ( where the text is landed on )
            double BindingDia = 0.5 * Dia;
            // Let pi = 3.1415, good enough
            double AvailableRad = TotalTitleWidth / ( 6.2832 * BindingDia );

            if ( !Blossom || AvailableRad < 1 )
            {
                double pDeg = 0;
                double TextOutset = BindingDia * ( 1 + 0.5 * IrisFactor );

                int Cyc = 1;
                // Outset the text after full ring of texts
                double CycOutset = ( Cyc - 1 ) * MaxTitleHeight;
                VisTitle.ForEach( ( C, D ) =>
                {
                    double CCenter = 0.5 * C.ActualWidth;

                    if ( ( Cyc * 360 - CCenter ) < pDeg )
                    {
                        Cyc++;
                        CycOutset = ( Cyc - 1 ) * MaxTitleHeight;
                    }

                    CompositeTransform Transform = C.RenderTransform as CompositeTransform;
                    Transform.TranslateX = -CCenter;
                    Transform.TranslateY = -MaxTitleHeight - TextOutset - CycOutset;
                    Transform.CenterY = TextOutset + MaxTitleHeight + CycOutset;
                    Transform.CenterX = CCenter;
                    Transform.Rotation = pDeg;

                    double NextWidth = D == null ? 0 : D.ActualWidth;
                    pDeg += 360 * AvailableRad * ( CCenter + 0.5 * NextWidth ) / TotalTitleWidth;
                } );
            }

            Canvas.SetTop( OuterRing, RingOrigin );
            Canvas.SetLeft( OuterRing, RingOrigin );
            Canvas.SetTop( ImageRing, ImageOrigin );
            Canvas.SetLeft( ImageRing, ImageOrigin );
        }

        public Task Explode()
        {
            if ( State == FloatyState.EXPLODE )
                return Task.Run( () => { } );

            State = FloatyState.EXPLODE;

            Stopped = new TaskCompletionSource<bool>();
            return Stopped.Task;
        }

        public Task Vanquish()
        {
            if ( State == FloatyState.VANQUISH )
                return Task.Run( () => { } );

            State = FloatyState.VANQUISH;

            Stopped = new TaskCompletionSource<bool>();
            return Stopped.Task;
        }

        public void Roam( double ox, double oy )
        {
            RenderTransform = new CompositeTransform() { TranslateX = ox, TranslateY = oy };
            ImageBrushy.Transform = new TranslateTransform() { X = -ox , Y = -oy };
        }

        private void ParametricCubic( ref double a, double b, double dx = 0.5, double dy = 0.5 )
        {
            a = dx * a + dy * b;
        }

        private void UpdateState()
        {
            Stop = false;
            CanRoam = true;

#if DEBUG
            Logger.Log( ID, string.Format( "Entering {0} state: {1}", State, Title ), LogType.DEBUG );
#endif
            switch ( State )
            {
                case FloatyState.EXPLODE:
                    TowardsRingDia = 0;
                    TowardsImageDia = BlossomFactor * ImageRingDia;
                    BlossomDia = TowardsImageDia;
                    return;
                case FloatyState.VANQUISH:
                    TowardsRingDia = ( 1 + IrisFactor ) * OuterRingDia;
                    TowardsImageDia = 0;
                    BlossomDia = TowardsRingDia;
                    return;
            }

            switch( State )
            {
                case FloatyState.MOUSE_OVER:
                    TowardsRingDia = OuterRingDia * ( 1 - 0.2 * IrisFactor );
                    TowardsImageDia = ImageRingDia * ( 1 + IrisFactor );
                    CanRoam = false;
                    break;

                case FloatyState.MOUSE_DOWN:
                    TowardsRingDia = OuterRingDia * ( 1 + IrisFactor );
                    TowardsImageDia = ImageRingDia * ( 1 - IrisFactor );
                    break;

                case FloatyState.NORMAL:
                default:
                    TowardsRingDia = OuterRingDia;
                    TowardsImageDia = ImageRingDia;
                    break;
            }

            StateChange();
        }

        private void InitializeComponents()
        {
            DiaUpdate();

            OuterRing = CreateCircle( 0, out RingOrigin );
            OuterRing.Fill = OuterRingBrush;

            ImageRing = CreateCircle( 0, out ImageOrigin );
            ImageRing.StrokeThickness = 0;
            ImageRing.Stroke = GetFloatyImage();

            Rings.Children.Add( OuterRing );
            Rings.Children.Add( ImageRing );

            RotateTransform RTransform = new RotateTransform();
            RTransform.Angle = NTimer.RandInt( 360 );
            RingText.RenderTransform = RTransform;

            CreateRotateStory();

            VisualUpdate();
        }

        private void CreateRotateStory()
        {
            if( RingRotateStory != null )
            {
                RingRotateStory.Stop();
                RingRotateStory.Children.Clear();
            }

            RingRotateStory = new Storyboard();

            double SAngle = ( RingText.RenderTransform as RotateTransform ).Angle;
            int AniLength = 50 + NTimer.RandInt( 50 );

            DoubleAnimationUsingKeyFrames d = new DoubleAnimationUsingKeyFrames();

            LinearDoubleKeyFrame StartAngle = new LinearDoubleKeyFrame();
            StartAngle.Value = SAngle;
            StartAngle.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromSeconds( 0 ) );

            LinearDoubleKeyFrame EndAngle = new LinearDoubleKeyFrame();
            EndAngle.Value = NTimer.RandChoice( -360 - SAngle, 360 + SAngle );
            EndAngle.KeyTime = KeyTime.FromTimeSpan( TimeSpan.FromSeconds( AniLength ) );

            d.Duration = new Duration( TimeSpan.FromSeconds( AniLength ) );
            d.RepeatBehavior = new RepeatBehavior( 100 );

            d.KeyFrames.Add( StartAngle );
            d.KeyFrames.Add( EndAngle );

            Storyboard.SetTarget( d, RingText );
            Storyboard.SetTargetProperty( d, "(UIElement.RenderTransform).(RotateTransform.Angle)" );
            RingRotateStory.Children.Add( d );

            RingRotateStory.Begin();
        }

        private Ellipse CreateCircle( double Diameter, out double Origin )
        {
            Ellipse Circle = new Ellipse();
            Circle.Width = Circle.Height = Diameter;

            Origin = 0.5 * ( CanvasDia - Diameter );
            Canvas.SetTop( Circle, Origin );
            Canvas.SetLeft( Circle, Origin );

            return Circle; 
        }

        private void ImageUpdate()
        {
            if ( ImageBrushy != null ) ImageBrushy.ImageSource = ImageSource;
        }

        private void OuterRingUpdate()
        {
            if ( OuterRing != null ) OuterRing.Fill = OuterRingBrush;
        }

        private void DiaUpdate()
        {
            double BaseDia = 0.5 * Width;
            CanvasDia = Width;
            OuterRingDia = BaseDia * ( 1 + 2 * IrisFactor );
            ImageRingDia = BaseDia * ( 1 - IrisFactor );

            // BaseDia is also the origin
            Canvas.SetTop( RingText, BaseDia );
            Canvas.SetLeft( RingText, BaseDia );
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Rings = ( Canvas ) GetTemplateChild( RingsName );
            RingText = ( Canvas ) GetTemplateChild( RingTextName );

            InitializeComponents();
        }

        private List<TextBlock> VisTitle;

        private void SimpleTextUpdate()
        {
            _rRDelta = TextSpeed;
            VisTitle?.ForEach( C => C.Foreground = TextBrush );
        }

        private void ComplexTextUpdate()
        {
            int l = Title.Length;

            VisTitle?.ForEach( ( x ) =>
            {
                RingText.Children.Remove( x );
            } );

            VisTitle = new List<TextBlock>();

            TextBlock Whole = NewTextBlock( Title );
            TotalTitleWidth = Whole.ActualWidth;
            MaxTitleHeight = 0;

            for ( int i = 0; i < l; i++ )
            {
                TextBlock C = NewTextBlock( Title[ i ] + "" );
                C.RenderTransform = new CompositeTransform();

                MaxTitleHeight = Math.Max( MaxTitleHeight, C.ActualHeight );

                VisTitle.Add( C );
                RingText.Children.Add( C );
            }
        }

        private void VisualUpdate()
        {
            if ( Rings == null ) return;
            InVisualUpdate = true;

            ComplexTextUpdate();
            UpdateState();

            InVisualUpdate = false;
        }


        private static void OnSimpleTextChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( FloatyButton ) d ).SimpleTextUpdate();
        }

        private static void OnTitleChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( FloatyButton ) d ).VisualUpdate();
        }

        private static void OnImageSourceChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( FloatyButton ) d ).ImageUpdate();
        }

        private static void OnOuterRingChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( FloatyButton ) d ).OuterRingUpdate();
        }

        private static void OnIrisFactorChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( FloatyButton ) d ).OuterRingUpdate();
        }

        protected override void OnPointerEntered( PointerRoutedEventArgs e )
        {
            base.OnPointerEntered( e );
            State = FloatyState.MOUSE_OVER;
        }

        protected override void OnPointerPressed( PointerRoutedEventArgs e )
        {
            base.OnPointerPressed( e );
            State = FloatyState.MOUSE_DOWN;
        }

        protected override void OnPointerExited( PointerRoutedEventArgs e )
        {
            base.OnPointerExited( e );
            State = FloatyState.NORMAL;
        }

        protected override void OnPointerReleased( PointerRoutedEventArgs e )
        {
            base.OnPointerReleased( e );
            State = FloatyState.EXPLODE;
        }

        protected override void OnPointerCaptureLost( PointerRoutedEventArgs e )
        {
            base.OnPointerCaptureLost( e );
            State = FloatyState.NORMAL;
        }

        private TextBlock NewTextBlock( string str )
        {
            TextBlock CharBlock = new TextBlock()
            {
                TextLineBounds = TextLineBounds.TrimToBaseline
            };

            CharBlock.Foreground = TextBrush;
            CharBlock.FontSize = 20;

            CharBlock.Text = str;
            if ( str == " " )
            {
                CharBlock.Text = "'";
                CharBlock.Measure( new Size( double.PositiveInfinity, double.PositiveInfinity ) );
                CharBlock.Arrange( new Rect( new Point(), CharBlock.DesiredSize ) );
                CharBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                CharBlock.Measure( new Size( double.PositiveInfinity, double.PositiveInfinity ) );
                CharBlock.Arrange( new Rect( new Point(), CharBlock.DesiredSize ) );
            }

            return CharBlock;
        }

        private ImageBrush GetFloatyImage()
        {
            ImageBrushy.Stretch = Stretch.None;
            ImageBrushy.ImageSource = ImageSource;

            return ImageBrushy;
        }

        private void StopGrowth()
        {
            Logger.Log( ID, "InAnimate: " + Title, LogType.DEBUG );
            Stop = true;
        }

        private void Deactivate()
        {
            ImageRing.Visibility
                = OuterRing.Visibility
                = Visibility.Collapsed;

            ImageRing.StrokeThickness
                = OuterRing.Width = OuterRing.Height
                = ImageRing.Width = ImageRing.Height
                = 0;

            StopGrowth();

            RingRotateStory.Stop();
            CanRoam = false;
            _state = FloatyState.NORMAL;
            Stopped?.TrySetResult( true );
        }
    }
}
