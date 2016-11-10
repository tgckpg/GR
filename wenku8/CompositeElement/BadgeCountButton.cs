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

    sealed class BadgeCountButton : AppBarButton
    {
        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
            "Count", typeof( int ), typeof( BadgeCountButton )
            , new PropertyMetadata( 0, OnCountChanged ) );

        public int Count
        {
            get { return ( int ) GetValue( CountProperty ); }
            set { SetValue( CountProperty, value ); }
        }

        private TextBlock CountText;
        private Border CountBadge;

        public BadgeCountButton()
            : base()
        {
            DefaultStyleKey = typeof( BadgeCountButton );
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

        private static void OnCountChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( BadgeCountButton ) d ).UpdateCount();
        }
    }
}