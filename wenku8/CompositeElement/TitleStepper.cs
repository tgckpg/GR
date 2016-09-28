using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

using Net.Astropenguin.Logging;

namespace wenku8.CompositeElement
{
    using Effects;
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

        private const string TRANSLATE_X = "(FrameworkElement.RenderTransform).(CompositeTransform.TranslateX)";
        private const string TRANSLATE_Y = "(FrameworkElement.RenderTransform).(CompositeTransform.TranslateY)";

        private double BlockHeight = 20;

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

        Storyboard NavStory;

        public void Prev()
        {
            if ( !Source.PrevStepAvailable() || NavStory?.GetCurrentState() == ClockState.Active ) return;
            NavStory?.Stop();

            NavStory = new Storyboard();

            SimpleStory.DoubleAnimation( NavStory, PrevTitle, TRANSLATE_Y, 0, BlockHeight );
            SimpleStory.DoubleAnimation( NavStory, CurrTitle, TRANSLATE_Y, BlockHeight, 2 * BlockHeight );
            SimpleStory.DoubleAnimation( NavStory, NextTitle, TRANSLATE_Y, 2 * BlockHeight, 3 * BlockHeight );

            SimpleStory.DoubleAnimation( NavStory, PrevTitle, "Opacity", 0.5, 1 );
            SimpleStory.DoubleAnimation( NavStory, CurrTitle, "Opacity", 1, 0.5 );
            SimpleStory.DoubleAnimation( NavStory, NextTitle, "Opacity", 0.5, 0 );

            NavStory.Completed += PrevStory_Completed;
            NavStory.Begin();
        }

        public void Next()
        {
            if ( !Source.NextStepAvailable() || NavStory?.GetCurrentState() == ClockState.Active ) return;
            NavStory?.Stop();

            NavStory = new Storyboard();

            SimpleStory.DoubleAnimation( NavStory, PrevTitle, TRANSLATE_Y, 0, -BlockHeight );
            SimpleStory.DoubleAnimation( NavStory, CurrTitle, TRANSLATE_Y, BlockHeight, 0 );
            SimpleStory.DoubleAnimation( NavStory, NextTitle, TRANSLATE_Y, 2 * BlockHeight, BlockHeight );

            SimpleStory.DoubleAnimation( NavStory, PrevTitle, "Opacity", 0.5, 0 );
            SimpleStory.DoubleAnimation( NavStory, CurrTitle, "Opacity", 1, 0.5 );
            SimpleStory.DoubleAnimation( NavStory, NextTitle, "Opacity", 0.5, 1 );

            NavStory.Completed += NextStory_Completed;
            NavStory.Begin();
        }

        private void PrevStory_Completed( object sender, object e )
        {
            Source.StepPrev();
            ResetTranslateY();

            PrevTitle.Opacity = 0;
            CurrTitle.Opacity = 1;
            NextTitle.Opacity = 0.5;

            UpdateDisplay();

            NavStory?.Stop();
            NavStory = new Storyboard();
            SimpleStory.DoubleAnimation( NavStory, PrevTitle, "Opacity", 0, 0.5 );
            NavStory.Begin();
        }

        private void NextStory_Completed( object sender, object e )
        {
            Source.StepNext();
            ResetTranslateY();

            PrevTitle.Opacity = 0.5;
            CurrTitle.Opacity = 1;
            NextTitle.Opacity = 0;

            UpdateDisplay();

            NavStory?.Stop();
            NavStory = new Storyboard();
            SimpleStory.DoubleAnimation( NavStory, NextTitle, "Opacity", 0, 0.5 );
            NavStory.Begin();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PrevTitle = ( TextBlock ) GetTemplateChild( PrevTitleName );
            CurrTitle = ( TextBlock ) GetTemplateChild( CurrTitleName );
            NextTitle = ( TextBlock ) GetTemplateChild( NextTitleName );

            BlockHeight = FontSize * 1.2;

            ResetTranslateY();

            UpdateDisplay();
        }

        private void ResetTranslateY()
        {
            ( ( CompositeTransform ) PrevTitle.RenderTransform ).TranslateY = 0;
            ( ( CompositeTransform ) CurrTitle.RenderTransform ).TranslateY = BlockHeight;
            ( ( CompositeTransform ) NextTitle.RenderTransform ).TranslateY = 2 * BlockHeight;
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
                EpisodeStepper ES = Source.Virtual();

                CurrTitle.Text = Source.EpTitle;

                if ( ES.StepPrev() )
                {
                    PrevTitle.Text = ES.EpTitle;
                    ES.StepNext();
                }

                if ( ES.StepNext() )
                    NextTitle.Text = ES.EpTitle;
            }
        }

    }
}