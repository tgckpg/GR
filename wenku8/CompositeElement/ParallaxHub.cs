using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Net.Astropenguin.Helpers;

namespace wenku8.CompositeElement
{
    // This class notifies whenever ScrollViewer offset changes
    public class ParallaxHub : Hub, INotifyPropertyChanged
    {
        public ScrollViewer RefSV;

        public static DependencyProperty ScrollerRibbonProperty = DependencyProperty.Register( "ScrollerRibbon", typeof( object ), typeof( ParallaxHub ), new PropertyMetadata( null ) );

        public object ScrollerRibbon
        {
            get { return GetValue( ScrollerRibbonProperty ); }
            set { SetValue( ScrollerRibbonProperty, value ); }
        }

        public ParallaxHub()
            :base()
        {
            DefaultStyleKey = typeof( ParallaxHub );
        }

        public delegate void ScrollViewerViewChangedHandler( object sender, ScrollViewerViewChangedEventArgs e );

        public event PropertyChangedEventHandler PropertyChanged;
        public event ScrollViewerViewChangedHandler ViewChanged;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RefSV = this.ChildAt<ScrollViewer>( 1 );

            RefSV.ViewChanged += RefSV_ViewChanged;
        }

        private void RefSV_ViewChanged( object sender, ScrollViewerViewChangedEventArgs e )
        {
            if ( ViewChanged != null )
                ViewChanged( this, e );

            if ( PropertyChanged != null )
                PropertyChanged( this, new PropertyChangedEventArgs( "HorizontalOffset" ) );
        }

        public double HorizontalOffset
        {
            get { return RefSV == null ? 0 : RefSV.HorizontalOffset; }
        }
    }
}
