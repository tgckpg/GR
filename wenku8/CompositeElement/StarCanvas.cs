using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Net.Astropenguin.Logging;

namespace wenku8.CompositeElement
{
    using Effects;
    using Model.Interfaces;

    public class StarCanvas : Canvas
    {
        public event TypedEventHandler<object, ItemClickedEventArgs> ItemClick;

        public sealed class ItemClickedEventArgs: EventArgs
        {
            public object ClickedItem { get; private set; }
            public object OriginalSource { get; private set; }

            internal ItemClickedEventArgs( object DataContext, object Source )
            {
                ClickedItem = DataContext;
                OriginalSource = Source;
            }
        }

        public static readonly string ID = typeof( StarCanvas ).Name;

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register( "ItemTemplate", typeof( DataTemplate ), typeof( StarCanvas ), new PropertyMetadata( null, OnDataTemplateChanged ) );
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register( "ItemsSource", typeof( object ), typeof( StarCanvas ), new PropertyMetadata( null, OnDataSourceChanged ) );

        public static readonly DependencyProperty MaxItemsAtOnceProperty = DependencyProperty.Register( "MaxItemsAtOnce", typeof( int ), typeof( StarCanvas ), new PropertyMetadata( 6, OnMaxItemsChanged ) );

        public static readonly DependencyProperty MinStarProperty = DependencyProperty.Register( "MinStar", typeof( double ), typeof( StarCanvas ), new PropertyMetadata( 200.0, OnStarSizeChanged ) );
        public static readonly DependencyProperty MaxStarProperty = DependencyProperty.Register( "MaxStar", typeof( double ), typeof( StarCanvas ), new PropertyMetadata( 400.0, OnStarSizeChanged ) );

        public DataTemplate ItemTemplate
        {
            get { return ( DataTemplate ) GetValue( ItemTemplateProperty ); }
            set { SetValue( ItemTemplateProperty, value ); }
        }

        public object ItemsSource
        {
            get { return GetValue( ItemsSourceProperty ); }
            set { SetValue( ItemsSourceProperty, value ); }
        }

        public int MaxItemsAtOnce
        {
            get { return ( int ) GetValue( MaxItemsAtOnceProperty ); }
            set { SetValue( MaxItemsAtOnceProperty, value ); }
        }

        public double MaxStar
        {
            get { return ( double ) GetValue( MaxStarProperty ); }
            set { SetValue( MaxStarProperty, value ); }
        }

        public double MinStar
        {
            get { return ( double ) GetValue( MinStarProperty ); }
            set { SetValue( MinStarProperty, value ); }
        }

        private Size SizeAvailable = Size.Empty;

        private static void OnDataTemplateChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( StarCanvas ) d ).DataTemplateUpdate();
        }

        private static void OnDataSourceChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( StarCanvas ) d ).DataSourceUpdate();
        }

        private static void OnMaxItemsChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( StarCanvas ) d ).GrowStars();
        }

        private static void OnStarSizeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( StarCanvas ) d ).GrowStars();
        }

        private void DataTemplateUpdate()
        {
            if ( ItemsSource == null ) return;
            SetItems();
        }

        private void DataSourceUpdate()
        {
            if ( ItemTemplate == null ) return;
            SetItems();
        }

        private volatile bool InitComplete = false;
        private volatile bool SlideShowing = false;

        private async void GrowStars()
        {
            if ( SlideShowing ) return;
            SlideShowing = true;

            await PurgeStars();

            IEnumerable<object> Items = ( IEnumerable<object> ) ItemsSource;
            if ( Items == null ) return;

            int l = Items.Count();

            ScrambleDist SDist = new ScrambleDist( MaxStar, MinStar, MaxItemsAtOnce );

            Width = SDist.Total;
            Logger.Log( ID, "Total Width: " + SDist.Total, LogType.DEBUG );

            double OffsetX = SDist.Total / MaxItemsAtOnce;

            List<object> Choices = new List<object>( Items );
            for( int i = 0; i < l && i < MaxItemsAtOnce; i ++ )
            {
                object o = AnimationTimer.RandChoiceFromList( Choices );
                Choices.Remove( o );

                FrameworkElement Element = ItemTemplate.LoadContent() as FrameworkElement;
                SDist.MoveNext();

                double StarScale = SDist.Current;
                Logger.Log( ID, "StarScale: " + StarScale, LogType.DEBUG );

                Element.Width = Element.Height = StarScale;

                Element.DataContext = o;
                Element.PointerReleased += ( sender, e ) =>
                {
                    ItemClick?.Invoke( this, new ItemClickedEventArgs( ( sender as FrameworkElement ).DataContext, sender ) );
                };

                SetLeft( Element, i * OffsetX );
                SetTop( Element, AnimationTimer.RandDouble( 0, SizeAvailable.Height - StarScale ) );

                Children.Add( Element );
            }

            // This is a slideshow
            if ( 0 < Choices.Count() )
            {
                Logger.Log( ID, "Cannot display every item. Enabling slideshow", LogType.DEBUG );
                await Task.Delay( 8000 );

                while( Children.Cast<IStar>().Any( C => C.Reacting ) )
                {
                    Logger.Log( ID, "User is interacting with the slideshow, waiting ...", LogType.DEBUG );
                    await Task.Delay( 5000 );
                }

                SlideShowing = false;
                GrowStars();
            }
            else
            {
                Logger.Log( ID, "All item can be present at once. Slide show is disabled", LogType.DEBUG );
                SlideShowing = false;
            }
        }

        private async Task PurgeStars()
        {
            int l = Children.Count();
            if ( l == 0 ) return;

            IStar LastStar = Children.Last() as IStar;
            if( 1 < l )
            {
                foreach ( UIElement StarElement in Children )
                {
                    IStar Star = StarElement as IStar;
                    if ( StarElement == LastStar ) break;

                    var j = Star.Vanquish();
                }
            }

            await LastStar.Vanquish();

            Children.Clear();
        }

        private void SetItems()
        {
            if ( InitComplete || SizeAvailable.Equals( Size.Empty ) ) return;

            IEnumerable<object> Items = ( IEnumerable<object> ) ItemsSource;
            if ( Items == null || Items.Count() == 0 ) return;

            InitComplete = true;

            GrowStars();
        }

        protected override Size MeasureOverride( Size availableSize )
        {
            SizeAvailable = availableSize;
            var j = Dispatcher.RunIdleAsync( ( x ) => SetItems() );

            return base.MeasureOverride( availableSize );
        }

    }

    internal class ScrambleDist : IEnumerator<double>
    {
        public static readonly string ID = typeof( ScrambleDist ).Name;
        private double _minVal;
        private double _maxVal;
        private double _totalSum;
        private int _total;
        private int _remaining;
        private double _blend;

        private double _totalQuota;
        private double _tol;

        private double _taken;

        object IEnumerator.Current { get { return Current; } }

        public double Current { get { return _minVal + _taken; } }
        public double Total { get { return _totalSum; } }

        public bool MoveNext()
        {
            if ( --_remaining == 0 )
            {
                _taken = _totalQuota;
                return false;
            }

            _taken = _tol < 0
                ? AnimationTimer.RandDouble( _blend + _tol )
                : AnimationTimer.RandDouble( _tol, _blend );

            _totalQuota -= _taken;

            _tol = 0.5 * _blend - _taken;

            return true;
        }

        public void Reset()
        {
            _blend = _maxVal - _minVal;
            _totalSum = ( _minVal + 0.5 * _blend ) * _total;

            _remaining = _total;
            _totalQuota = 0.5 * _blend * _total;
            _tol = 0;
        }

        public void Dispose()
        {
            // Nothing to do
        }

        public ScrambleDist( double minValue, double maxValue, int numSamples )
        {
            _minVal = minValue;
            _maxVal = maxValue;
            _total = numSamples;

            Reset();
        }
    }
}
