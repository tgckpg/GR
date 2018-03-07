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

namespace GR.CompositeElement
{
	using Effects;
	using Model.Book;
	using Net.Astropenguin.UI;

	[TemplatePart( Name = PrevTitleName, Type = typeof( FrameworkElement ) )]
	[TemplatePart( Name = CurrTitleName, Type = typeof( FrameworkElement ) )]
	[TemplatePart( Name = NextTitleName, Type = typeof( FrameworkElement ) )]
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

		private string TRANSLATE
		{
			get { return ( PrevTitle is TextBlock ) ? TRANSLATE_Y : TRANSLATE_X; }
		}

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

		private FrameworkElement PrevTitle;
		private FrameworkElement CurrTitle;
		private FrameworkElement NextTitle;

		private string PrevTitleText
		{
			get
			{
				if ( PrevTitle is TextBlock ) return ( ( TextBlock ) PrevTitle ).Text;
				return ( ( VerticalStack ) PrevTitle ).Text;
			}
			set
			{
				if ( PrevTitle is TextBlock ) ( ( TextBlock ) PrevTitle ).Text = value;
				else ( ( VerticalStack ) PrevTitle ).Text = value;
			}
		}

		private string CurrTitleText
		{
			get
			{
				if ( CurrTitle is TextBlock ) return ( ( TextBlock ) CurrTitle ).Text;
				return ( ( VerticalStack ) CurrTitle ).Text;
			}
			set
			{
				if ( CurrTitle is TextBlock ) ( ( TextBlock ) CurrTitle ).Text = value;
				else ( ( VerticalStack ) CurrTitle ).Text = value;
			}
		}

		private string NextTitleText
		{
			get
			{
				if ( NextTitle is TextBlock ) return ( ( TextBlock ) NextTitle ).Text;
				return ( ( VerticalStack ) NextTitle ).Text;
			}
			set
			{
				if ( NextTitle is TextBlock ) ( ( TextBlock ) NextTitle ).Text = value;
				else ( ( VerticalStack ) NextTitle ).Text = value;
			}
		}

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

			SimpleStory.DoubleAnimation( NavStory, PrevTitle, TRANSLATE, 0, BlockHeight );
			SimpleStory.DoubleAnimation( NavStory, CurrTitle, TRANSLATE, BlockHeight, 2 * BlockHeight );
			SimpleStory.DoubleAnimation( NavStory, NextTitle, TRANSLATE, 2 * BlockHeight, 3 * BlockHeight );

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

			SimpleStory.DoubleAnimation( NavStory, PrevTitle, TRANSLATE, 0, -BlockHeight );
			SimpleStory.DoubleAnimation( NavStory, CurrTitle, TRANSLATE, BlockHeight, 0 );
			SimpleStory.DoubleAnimation( NavStory, NextTitle, TRANSLATE, 2 * BlockHeight, BlockHeight );

			SimpleStory.DoubleAnimation( NavStory, PrevTitle, "Opacity", 0.5, 0 );
			SimpleStory.DoubleAnimation( NavStory, CurrTitle, "Opacity", 1, 0.5 );
			SimpleStory.DoubleAnimation( NavStory, NextTitle, "Opacity", 0.5, 1 );

			NavStory.Completed += NextStory_Completed;
			NavStory.Begin();
		}

		private void PrevStory_Completed( object sender, object e )
		{
			if ( Mode == StepMode.EP ) Source.StepPrev();
			else Source.StepPrevVol();

			ResetTranslate();

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
			if ( Mode == StepMode.EP ) Source.StepNext();
			else Source.StepNextVol();

			ResetTranslate();

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

			PrevTitle = ( FrameworkElement ) GetTemplateChild( PrevTitleName );
			CurrTitle = ( FrameworkElement ) GetTemplateChild( CurrTitleName );
			NextTitle = ( FrameworkElement ) GetTemplateChild( NextTitleName );

			BlockHeight = FontSize * 1.2;

			ResetTranslate();

			UpdateDisplay();
		}

		private void ResetTranslate()
		{
			if ( PrevTitle is TextBlock )
			{
				( ( CompositeTransform ) PrevTitle.RenderTransform ).TranslateY = 0;
				( ( CompositeTransform ) CurrTitle.RenderTransform ).TranslateY = BlockHeight;
				( ( CompositeTransform ) NextTitle.RenderTransform ).TranslateY = 2 * BlockHeight;
			}
			else
			{
				( ( CompositeTransform ) PrevTitle.RenderTransform ).TranslateX = 0;
				( ( CompositeTransform ) CurrTitle.RenderTransform ).TranslateX = BlockHeight;
				( ( CompositeTransform ) NextTitle.RenderTransform ).TranslateX = 2 * BlockHeight;
			}
		}

		private static void OnPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			( ( TitleStepper ) d ).UpdateDisplay();
		}

		public void UpdateDisplay()
		{
			if ( Source == null || NextTitle == null ) return;

			PrevTitleText = CurrTitleText = NextTitleText = "";

			if ( Mode == StepMode.VOL )
			{
				PrevTitleText = Source.PrevVolTitle;
				CurrTitleText = Source.VolTitle;
				NextTitleText = Source.NextVolTitle;
			}
			else
			{
				EpisodeStepper ES = Source.Virtual();

				CurrTitleText = Source.EpTitle;

				if ( ES.StepPrev() )
				{
					PrevTitleText = ES.EpTitle;
					ES.StepNext();
				}

				if ( ES.StepNext() )
					NextTitleText = ES.EpTitle;
			}
		}

	}
}