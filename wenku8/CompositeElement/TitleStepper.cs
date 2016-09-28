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
    using Model.Book;

    [TemplatePart( Name = PrevTitleName, Type = typeof( TextBlock ) )]
    [TemplatePart( Name = CurrTitleName, Type = typeof( TextBlock ) )]
    [TemplatePart( Name = NextTitleName, Type = typeof( TextBlock ) )]
    public class TitleStepper : Control
    {
        public static readonly string ID = typeof( TitleStepper ).Name;

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register( "Mode", typeof( StepMode ), typeof( TitleStepper ), new PropertyMetadata( StepMode.VOL, OnPropertyChanged ) );
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register( "Source", typeof( EpisodeStepper ), typeof( TitleStepper ), new PropertyMetadata( null, OnPropertyChanged ) );

        private const string PrevTitleName = "PrevTitle";
        private const string CurrTitleName = "CurrTitle";
        private const string NextTitleName = "NextTitle";

        public StepMode Mode
        {
            get { return ( StepMode ) GetValue( ModeProperty ); }
            set { SetValue( ModeProperty, value ); }
        }

        public EpisodeStepper Source
        {
            get { return ( EpisodeStepper ) GetValue( SourceProperty ); }
            set { SetValue( SourceProperty, value ); }
        }

        public enum StepMode { VOL, EP };

        private TextBlock PrevTitle;
        private TextBlock CurrTitle;
        private TextBlock NextTitle;

        public TitleStepper()
            : base()
        {
            DefaultStyleKey = typeof( TitleStepper );
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PrevTitle = ( TextBlock ) GetTemplateChild( PrevTitleName );
            CurrTitle = ( TextBlock ) GetTemplateChild( CurrTitleName );
            NextTitle = ( TextBlock ) GetTemplateChild( NextTitleName );

            UpdateDisplay();
        }

        private static void OnPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( TitleStepper ) d ).UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if ( Source == null || NextTitle == null ) return;

            PrevTitle.Text = CurrTitle.Text = NextTitle.Text = "";

            if ( Mode == StepMode.VOL )
            {
                PrevTitle.Text = Source.PrevVolTitle;
                CurrTitle.Text = Source.VolTitle;
                NextTitle.Text = Source.NextVolTitle;
            }
            else
            {
                CurrTitle.Text = Source.EpTitle;

                EpisodeStepper ES = Source.Virtual();

                if ( ES.StepPrev() )
                    PrevTitle.Text = ES.EpTitle;

                if ( ES.StepNext() && ES.StepNext() )
                    NextTitle.Text = ES.EpTitle;
            }
        }

    }
}