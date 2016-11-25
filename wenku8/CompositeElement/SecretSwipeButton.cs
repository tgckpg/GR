using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace wenku8.CompositeElement
{
    using Effects;
    using Resources;

    sealed class SecretSwipeButton : SecondaryIconButton
    {
        public delegate void IndexUpdate( object sender, int Index );
        public event IndexUpdate OnIndexUpdate;

        public static readonly DependencyProperty Glyph2Property = DependencyProperty.Register(
            "Glyph2", typeof( string ), typeof( SecondaryIconButton )
            , new PropertyMetadata( SegoeMDL2.Accept, OnGlyphChanged ) );

        public string Glyph2
        {
            get { return ( string ) GetValue( Glyph2Property ); }
            set { SetValue( Glyph2Property, value ); }
        }

        public static readonly DependencyProperty Label2Property = DependencyProperty.Register(
            "Label2", typeof( string ), typeof( SecondaryIconButton )
            , new PropertyMetadata( "Label 2", OnLabelChanged ) );

        public string Label2
        {
            get { return ( string ) GetValue( Label2Property ); }
            set { SetValue( Label2Property, value ); }
        }

        public int Index { get; private set; }

        private double ZoomTrigger = 0;

        private double VT = 50;

        private string Label1;
        private string Glyph1;

        Grid RootGrid;
        TranslateTransform CGTransform;

        Storyboard ContentRestore;
        Storyboard ContentAway;

        public SecretSwipeButton( string Glyph )
            : base( Glyph )
        {
            DependencyProperty TWidthProperty =
                DependencyProperty.RegisterAttached( "TLabel", typeof( double ),
                typeof( FloatyButton ), new PropertyMetadata( 0.0, OnLabelChanged ) );
            Binding B = new Binding();
            B.Path = new PropertyPath( "Label" );
            B.Source = this;
            SetBinding( TWidthProperty, B );
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Label1 = Label;

            RootGrid = ( Grid ) GetTemplateChild( "Root" );

            CGTransform = new TranslateTransform();
            RootGrid.RenderTransform = CGTransform;

            ContentRestore = new Storyboard();

            RootGrid.ManipulationCompleted += VEManipulationEndX;
            RootGrid.ManipulationDelta += VEZoomBack;

            RootGrid.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateRailsX;
            RootGrid.ManipulationStarted += VEManiStart;
        }

        private void RestorePosition()
        {
            ContentRestore.Stop();
            ContentRestore.Children.Clear();
            SimpleStory.DoubleAnimation( ContentRestore, CGTransform, "X", CGTransform.X, 0 );

            ContentRestore.Begin();
        }

        protected override void UpdateGlyph()
        {
            base.UpdateGlyph();

            if ( Index == 1 ) Glyph2 = Glyph;
            else Glyph1 = Glyph;
        }

        private static void OnLabelChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( SecretSwipeButton ) d ).UpdateLabel();
        }

        private void UpdateLabel()
        {
            if ( Index == 1 ) Label2 = Label;
            else Label1 = Label;
        }

        private void VEManiStart( object sender, ManipulationStartedRoutedEventArgs e )
        {
            CGTransform.SetValue( TranslateTransform.XProperty, CGTransform.GetValue( TranslateTransform.XProperty ) );
            ContentRestore.Stop();
        }

        private void VEZoomBack( object sender, ManipulationDeltaRoutedEventArgs e )
        {
            CGTransform.X += e.Delta.Translation.X;
            ZoomTrigger += e.Delta.Translation.X;
        }

        private void VEManipulationEndX( object sender, ManipulationCompletedRoutedEventArgs e )
        {
            double dv = e.Cumulative.Translation.X;
            ContentAway?.Stop();
            if ( VT < dv )
            {
                ContentAway = new Storyboard();
                SimpleStory.DoubleAnimation(
                    ContentAway, CGTransform, "X"
                    , CGTransform.X
                    , RootGrid.ActualWidth );

                BeginContentAway();
            }
            else if ( dv < -VT )
            {
                ContentAway = new Storyboard();
                SimpleStory.DoubleAnimation(
                    ContentAway, CGTransform, "X"
                    , CGTransform.X
                    , -RootGrid.ActualWidth );

                BeginContentAway();
            }
            else
            {
                RestorePosition();
            }
        }

        private void BeginContentAway()
        {
            ContentAway.Begin();
            ContentAway.Completed += ( s, e ) =>
            {
                ContentAway.Stop();
                CGTransform.X = -( double ) CGTransform.GetValue( TranslateTransform.XProperty );
                RestorePosition();
                SwapLables();
            };
        }

        private void SwapLables()
        {
            if ( Index == 0 )
            {
                Index = 1;
                Label = Label2;
                Glyph = Glyph2;
            }
            else
            {
                Index = 0;
                Label = Label1;
                Glyph = Glyph1;
            }

            OnIndexUpdate?.Invoke( this, Index );
        }
    }
}