using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace wenku8.Effects
{
    public enum TransitionState
    {
        Hidden,
        Active,
        Inactive
    }
    public enum TransitionMode
    {
        A01,
        A01_X_N30_0,
        A01_X_0_N30,
        A01_X_30_0,
        A01_X_0_30,
        A01_X_N30_30,
        A01_X_30_N30,
        A01_X_30_30,
        A01_X_N30_N30,

        A01_Y_N30_0,
        A01_Y_0_N30,
        A01_Y_30_0,
        A01_Y_0_30,
        A01_Y_N30_30,
        A01_Y_30_N30,
        A01_Y_30_30,
        A01_Y_N30_N30,
    }

    public sealed class TransitionDisplay
    {
        // Storyboard
        public static Storyboard GetStoryboard( DependencyObject d ) { return ( Storyboard ) d.GetValue( StoryboardProperty ); }
        public static void SetStoryboard( DependencyObject d, Storyboard Storyboard ) { d.SetValue( StoryboardProperty, Storyboard ); }

        public static readonly DependencyProperty StoryboardProperty =
            DependencyProperty.RegisterAttached( "Storyboard", typeof( Storyboard ),
            typeof( TransitionDisplay ), new PropertyMetadata( null ) );

        // Mode
        public static TransitionMode GetMode( DependencyObject d ) { return ( TransitionMode ) d.GetValue( ModeProperty ); }
        public static void SetMode( DependencyObject d, TransitionMode Mode ) { d.SetValue( ModeProperty, Mode ); }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.RegisterAttached( "Mode", typeof( TransitionMode ),
            typeof( TransitionDisplay ), new PropertyMetadata( TransitionMode.A01_Y_N30_0 ) );

        // UseVisibility
        public static bool GetUseVisibility( DependencyObject d ) { return ( bool ) d.GetValue( UseVisibilityProperty ); }
        public static void SetUseVisibility( DependencyObject d, bool UseVisibility ) { d.SetValue( UseVisibilityProperty, UseVisibility ); }

        public static readonly DependencyProperty UseVisibilityProperty =
            DependencyProperty.RegisterAttached( "UseVisibility", typeof( bool ),
            typeof( TransitionDisplay ), new PropertyMetadata( true ) );

        // State
        public static TransitionState GetState( DependencyObject d ) { return ( TransitionState ) d.GetValue( StateProperty ); }
        public static void SetState( DependencyObject d, TransitionState State ) { d.SetValue( StateProperty, State ); }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached( "State", typeof( TransitionState ),
            typeof( TransitionDisplay ), new PropertyMetadata( TransitionState.Hidden, OnStatePropertyChanged ) );

        private static void OnStatePropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if ( e.OldValue.Equals( e.NewValue ) ) return;

            ApplyStoryboard( ( TransitionState ) e.OldValue, ( TransitionState ) e.NewValue, d as FrameworkElement )?.Begin();
        }

        private static Storyboard ApplyStoryboard( TransitionState OState, TransitionState State, FrameworkElement Elem )
        {
            double t = 350;
            if ( OState == TransitionState.Hidden )
            {
                t = 0;
            }

            Storyboard Sb = GetStoryboard( Elem );
            if ( Sb == null )
            {
                Sb = new Storyboard();
            }
            else
            {
                Sb.Stop();
                Sb.Children.Clear();
            }

            TransitionMode Mode = GetMode( Elem );
            TransStruct TStruct = new TransStruct() { Elem = Elem, Sb = Sb, State = State };

            if ( GetUseVisibility( Elem ) )
            {
                if ( TStruct.State == TransitionState.Active )
                {
                    SimpleStory.ObjectAnimation( TStruct.Sb, TStruct.Elem, "Visibility", Visibility.Collapsed, Visibility.Visible, 0 );
                }
                else
                {
                    SimpleStory.ObjectAnimation( TStruct.Sb, TStruct.Elem, "Visibility", Visibility.Visible, Visibility.Collapsed );
                }
            }

            switch ( Mode )
            {
                case TransitionMode.A01:
                    if ( State == TransitionState.Active )
                    {
                        SimpleStory.DoubleAnimation( Sb, Elem, "Opacity", 0, 1, t );
                    }
                    else if ( State == TransitionState.Inactive )
                    {
                        SimpleStory.DoubleAnimation( Sb, Elem, "Opacity", 1, 0, t );
                    }
                    goto NormalTrans;

                case TransitionMode.A01_X_N30_0: TStruct.FromTo( -30, 0 ); goto TransX;
                case TransitionMode.A01_X_0_N30: TStruct.FromTo( 0, -30 ); goto TransX;
                case TransitionMode.A01_X_30_0: TStruct.FromTo( 30, 0 ); goto TransX;
                case TransitionMode.A01_X_0_30: TStruct.FromTo( 0, 30 ); goto TransX;
                case TransitionMode.A01_X_N30_30: TStruct.FromTo( -30, 30 ); goto TransX;
                case TransitionMode.A01_X_30_N30: TStruct.FromTo( 30, -30 ); goto TransX;
                case TransitionMode.A01_X_30_30: TStruct.FromTo( 30, 30 ); goto TransX;
                case TransitionMode.A01_X_N30_N30: TStruct.FromTo( -30, -30 ); goto TransX;

                case TransitionMode.A01_Y_N30_0: TStruct.FromTo( -30, 0 ); goto TransY;
                case TransitionMode.A01_Y_0_N30: TStruct.FromTo( 0, -30 ); goto TransY;
                case TransitionMode.A01_Y_30_0: TStruct.FromTo( 30, 0 ); goto TransY;
                case TransitionMode.A01_Y_0_30: TStruct.FromTo( 0, 30 ); goto TransY;
                case TransitionMode.A01_Y_N30_30: TStruct.FromTo( -30, 30 ); goto TransY;
                case TransitionMode.A01_Y_30_N30: TStruct.FromTo( 30, -30 ); goto TransY;
                case TransitionMode.A01_Y_30_30: TStruct.FromTo( 30, 30 ); goto TransY;
                case TransitionMode.A01_Y_N30_N30: TStruct.FromTo( -30, -30 ); goto TransY;
            }

            TransX:
            TStruct.Prop = "(FrameworkElement.RenderTransform).(TranslateTransform.X)";
            goto AxisTrans;

            TransY:
            TStruct.Prop = "(FrameworkElement.RenderTransform).(TranslateTransform.Y)";
            goto AxisTrans;

            AxisTrans:
            TransAxis( TStruct, t );

            NormalTrans:
            SetStoryboard( Elem, Sb );
            return Sb;
        }

        private static void TransAxis( TransStruct TStruct, double t )
        {
            TStruct.Elem.RenderTransform = new TranslateTransform();
            if ( TStruct.State == TransitionState.Active )
            {
                SimpleStory.DoubleAnimation( TStruct.Sb, TStruct.Elem, TStruct.Prop, TStruct.ActiveFrom, 0, t );
                SimpleStory.DoubleAnimation( TStruct.Sb, TStruct.Elem, "Opacity", 0, 1, t );
            }
            else if ( TStruct.State == TransitionState.Inactive )
            {
                SimpleStory.DoubleAnimation( TStruct.Sb, TStruct.Elem, TStruct.Prop, 0, TStruct.InactiveTo, t );
                SimpleStory.DoubleAnimation( TStruct.Sb, TStruct.Elem, "Opacity", 1, 0, t );
            }
        }

        private class TransStruct
        {
            public FrameworkElement Elem;
            public TransitionState State;
            public Storyboard Sb;
            public string Prop;
            public double ActiveFrom { get; private set; }
            public double InactiveTo { get; private set; }

            public void FromTo( double Active, double Inactive )
            {
                ActiveFrom = Active;
                InactiveTo = Inactive;
            }
        }

    }
}