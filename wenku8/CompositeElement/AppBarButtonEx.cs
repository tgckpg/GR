using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace wenku8.CompositeElement
{
    using Effects;

    sealed class AppBarButtonEx : AppBarButton
    {
        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
            "Count", typeof( int ), typeof( AppBarButtonEx )
            , new PropertyMetadata( 0, OnCountChanged ) );

        public int Count
        {
            get { return ( int ) GetValue( CountProperty ); }
            set { SetValue( CountProperty, value ); }
        }

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
            "IsLoading", typeof( bool ), typeof( AppBarButtonEx )
            , new PropertyMetadata( false, OnIsLoadingChanged ) );

        public bool IsLoading
        {
            get { return ( bool ) GetValue( IsLoadingProperty ); }
            set { SetValue( IsLoadingProperty, value ); }
        }

        private TextBlock CountText;
        private Border CountBadge;

        public AppBarButtonEx()
            : base()
        {
            DefaultStyleKey = typeof( AppBarButtonEx );
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CountText = ( TextBlock ) GetTemplateChild( "CountText" );
            CountBadge = ( Border ) GetTemplateChild( "CountBadge" );
            TransitionDisplay.SetMode( CountBadge, TransitionMode.A01_Y_30_30 );

            UpdateCount();
        }

        private void UpdateCount()
        {
            if ( CountText == null ) return;
            TransitionDisplay.SetState( CountBadge, 0 < Count ? TransitionState.Active : TransitionState.Inactive );

            CountText.Text = Count.ToString();
        }

        private void UpdateLoadingState()
        {
            IsEnabled = !IsLoading;
        }

        private static void OnCountChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( AppBarButtonEx ) d ).UpdateCount();
        }

        private static void OnIsLoadingChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( AppBarButtonEx ) d ).UpdateLoadingState();
        }
    }
}