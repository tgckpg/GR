using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

using Net.Astropenguin.Logging;
using Windows.UI.Xaml.Data;

namespace wenku8.CompositeElement
{
	public enum PaneStates
	{
		Opened, Closed
	}

	[TemplateVisualState( Name = "Opened", GroupName = "PaneStates" )]
	[TemplateVisualState( Name = "Closed", GroupName = "PaneStates" )]
	[TemplatePart( Name = PaneGridName, Type = typeof( Grid ) )]
	[TemplatePart( Name = PaneContentName, Type = typeof( Grid ) )]
	[TemplatePart( Name = SwipeDetectName, Type = typeof( Rectangle ) )]
	public sealed class PassiveSplitView : Control
	{
		public static readonly string ID = typeof( PassiveSplitView ).Name;

		private const string PaneGridName = "InfoPane";
		private const string PaneContentName = "PaneContent";
		private const string SwipeDetectName = "SwipeGesture";

		public static readonly DependencyProperty ManiModeProperty
			= DependencyProperty.Register(
				"ManiMode"
				, typeof( ManipulationModes )
				, typeof( PassiveSplitView )
				, new PropertyMetadata( ManipulationModes.TranslateX, OnManiModeChanged )
			);

		public static readonly DependencyProperty PaneProperty
			= DependencyProperty.Register(
				"Pane"
				, typeof( object )
				, typeof( PassiveSplitView )
				, new PropertyMetadata( null )
			);

		public static readonly DependencyProperty ContentProperty
			= DependencyProperty.Register(
				"Content"
				, typeof( object )
				, typeof( PassiveSplitView )
				, new PropertyMetadata( null )
			);

		public static readonly DependencyProperty PanelWidthProperty
			= DependencyProperty.Register(
				"PanelWidth"
				, typeof( double )
				, typeof( PassiveSplitView )
				, new PropertyMetadata( null )
			);

		public static readonly DependencyProperty EnablePaneSwipeProperty
			= DependencyProperty.Register(
				"EnablePaneSwipe"
				, typeof( bool )
				, typeof( PassiveSplitView )
				, new PropertyMetadata( null )
			);

		public static readonly DependencyProperty PanelBackgroundProperty
			= DependencyProperty.Register(
				"PanelBackground"
				, typeof( Brush )
				, typeof( PassiveSplitView )
				, new PropertyMetadata( null )
			);

		public static readonly DependencyProperty StateProperty
			= DependencyProperty.Register(
				"State"
				, typeof( PaneStates )
				, typeof( PassiveSplitView )
				, new PropertyMetadata( PaneStates.Closed, OnPaneStateChanged )
			);

		public Grid PaneGrid { get; private set; }
		private ContentPresenter PaneContent { get; set; }
		private Rectangle SwipeDetect { get; set; }

		private bool ModeX = true;

		public FocusState PaneFocusState = FocusState.Unfocused;

		private static void OnPaneStateChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			( ( PassiveSplitView ) d ).UpdateVisualState( true );
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			PaneGrid = ( Grid ) GetTemplateChild( PaneGridName );
			PaneContent = ( ContentPresenter ) GetTemplateChild( PaneContentName );
			SwipeDetect = ( Rectangle ) GetTemplateChild( SwipeDetectName );

			PaneGrid.GotFocus += ( a, b ) => { Logger.Log( ID, "Pane Got Focus" ); PaneFocusState = FocusState.Pointer; };
			PaneGrid.LostFocus += ( a, b ) => { PaneFocusState = FocusState.Unfocused; };

			EnSwipe( EnablePaneSwipe );
		}

		private void EnSwipe( bool Enable )
		{
			if ( SwipeDetect == null ) return;
			SwipeDetect.ManipulationStarted -= SwipeDetect_ManipulationStarted;
			SwipeDetect.ManipulationDelta -= PaneGrid_ManipulationDeltaX;
			SwipeDetect.ManipulationDelta -= PaneGrid_ManipulationDeltaNX;
			SwipeDetect.ManipulationDelta -= PaneGrid_ManipulationDeltaY;
			SwipeDetect.ManipulationCompleted -= SwipeDetect_ManipulationCompleted;

			SwipeDetect.Visibility = Enable ? Visibility.Visible : Visibility.Collapsed;
			if ( Enable )
			{
				UpdateGestureArea();

				SwipeDetect.ManipulationStarted += SwipeDetect_ManipulationStarted;

				if ( ModeX )
				{
					if ( FlowDirection == FlowDirection.RightToLeft )
						SwipeDetect.ManipulationDelta += PaneGrid_ManipulationDeltaNX;
					else SwipeDetect.ManipulationDelta += PaneGrid_ManipulationDeltaX;
				}
				else
				{
					SwipeDetect.ManipulationDelta += PaneGrid_ManipulationDeltaY;
				}

				SwipeDetect.ManipulationCompleted += SwipeDetect_ManipulationCompleted;
			}
		}

		private void SwipeDetect_ManipulationStarted( object sender, ManipulationStartedRoutedEventArgs e )
		{
			CompositeTransform PGTransform = ( CompositeTransform ) PaneGrid.RenderTransform;

			if( ModeX )
			{
				PGTransform.TranslateX = ( double ) PGTransform.GetValue( CompositeTransform.TranslateXProperty );
				PGTransform.TranslateY = 0;
			}
			else
			{
				PGTransform.TranslateX = 0;
				PGTransform.TranslateY = ( double ) PGTransform.GetValue( CompositeTransform.TranslateYProperty );
			}

			VisualStateManager.GoToState( this, "Manip", false );
		}

		private void SwipeDetect_ManipulationCompleted( object sender, ManipulationCompletedRoutedEventArgs e )
		{
			CompositeTransform PaneTransform = ( CompositeTransform ) PaneGrid.RenderTransform;
			bool Open = ModeX
				? ( -( .75 * PaneGrid.ActualWidth ) < PaneTransform.TranslateX )
				: ( -( .75 * PaneGrid.ActualHeight ) < PaneTransform.TranslateY )
				;
			if ( Open )
			{
				State = PaneStates.Opened;
			}
			else
			{
				State = PaneStates.Closed;
			}
		}

		private void UpdateGestureArea()
		{
			if ( SwipeDetect == null ) return;

			SwipeDetect.ManipulationMode = ManiMode;

			if ( ModeX )
			{
				SwipeDetect.Width = 35;
				SwipeDetect.Height = double.NaN;
				SwipeDetect.VerticalAlignment = VerticalAlignment.Stretch;
				SwipeDetect.HorizontalAlignment = HorizontalAlignment.Left;
			}
			else
			{
				SwipeDetect.Width = double.NaN;
				SwipeDetect.Height = 35;
				SwipeDetect.VerticalAlignment = VerticalAlignment.Top;
				SwipeDetect.HorizontalAlignment = HorizontalAlignment.Stretch;
			}
		}

		private void SetManiMode( ManipulationModes From, ManipulationModes To )
		{
			ModeX = ( To == ManipulationModes.TranslateX );
			EnSwipe( EnablePaneSwipe );

			// XY
			if ( From == ManipulationModes.TranslateX && !ModeX )
			{
				VisualStateManager.GoToState( this, ( State == PaneStates.Opened ) ? "HOpened" : "HClosed", false );
			}
			// YX
			else if ( From == ManipulationModes.TranslateY && ModeX )
			{
				VisualStateManager.GoToState( this, ( State == PaneStates.Opened ) ? "Opened" : "Closed", false );
			}
		}

		private void SetFlowDirection()
		{
			if ( ModeX && SwipeDetect != null )
			{
				SwipeDetect.ManipulationDelta -= PaneGrid_ManipulationDeltaX;
				SwipeDetect.ManipulationDelta -= PaneGrid_ManipulationDeltaNX;

				if ( FlowDirection == FlowDirection.RightToLeft )
					SwipeDetect.ManipulationDelta += PaneGrid_ManipulationDeltaNX;
				else SwipeDetect.ManipulationDelta += PaneGrid_ManipulationDeltaX;
			}
		}

		private static void OnFlowDirectionChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			( ( PassiveSplitView ) d ).SetFlowDirection();
		}

		private static void OnManiModeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			( ( PassiveSplitView ) d ).SetManiMode( ( ManipulationModes ) e.OldValue, ( ManipulationModes ) e.NewValue );
		}

		private void PaneGrid_ManipulationDeltaX( object sender, ManipulationDeltaRoutedEventArgs e )
		{
			CompositeTransform PaneTransform = ( CompositeTransform ) PaneGrid.RenderTransform;
			PaneTransform.TranslateX += e.Delta.Translation.X;

			if ( 0 < PaneTransform.TranslateX )
			{
				PaneTransform.TranslateX = 0;
			}
			else if ( PaneTransform.TranslateX < -PaneGrid.ActualWidth )
			{
				PaneTransform.TranslateX = -PaneGrid.ActualWidth;
			}

			UpdatePresenter( PaneTransform.TranslateX / -PaneGrid.ActualWidth );
		}

		private void PaneGrid_ManipulationDeltaNX( object sender, ManipulationDeltaRoutedEventArgs e )
		{
			CompositeTransform PaneTransform = ( CompositeTransform ) PaneGrid.RenderTransform;
			PaneTransform.TranslateX -= e.Delta.Translation.X;

			if ( 0 < PaneTransform.TranslateX )
			{
				PaneTransform.TranslateX = 0;
			}
			else if ( PaneTransform.TranslateX < -PaneGrid.ActualWidth )
			{
				PaneTransform.TranslateX = -PaneGrid.ActualWidth;
			}

			UpdatePresenter( PaneTransform.TranslateX / -PaneGrid.ActualWidth );
		}

		private void PaneGrid_ManipulationDeltaY( object sender, ManipulationDeltaRoutedEventArgs e )
		{
			CompositeTransform PaneTransform = ( CompositeTransform ) PaneGrid.RenderTransform;
			PaneTransform.TranslateY += e.Delta.Translation.Y;

			if ( 0 < PaneTransform.TranslateY )
			{
				PaneTransform.TranslateY = 0;
			}
			else if ( PaneTransform.TranslateY < -PaneGrid.ActualHeight )
			{
				PaneTransform.TranslateY = -PaneGrid.ActualHeight;
			}
			UpdatePresenter( PaneTransform.TranslateY / -PaneGrid.ActualHeight );
		}

		private void UpdatePresenter( double v )
		{
			CompositeTransform ContentTrans = PaneContent.RenderTransform as CompositeTransform;
			if ( ModeX )
			{
				ContentTrans.TranslateX = 50 * v;
			}
			else
			{
				ContentTrans.TranslateY = 50 * v;
			}

			PaneContent.Opacity = 1 - v;

			PaneContent.UpdateLayout();
		}

		private void UpdateVisualState( bool UseTransition )
		{
			switch ( State )
			{
				case PaneStates.Opened:
					VisualStateManager.GoToState( this, ModeX ? "Opened" : "HOpened", UseTransition );
					break;
				default:
				case PaneStates.Closed:
					VisualStateManager.GoToState( this, ModeX ? "Closed" : "HClosed", UseTransition );
					break;
			}
			Logger.Log( ID, string.Format( "State is {0}", State ) );
		}

		public ManipulationModes ManiMode
		{
			get { return ( ManipulationModes ) GetValue( ManiModeProperty ); }
			set { SetValue( ManiModeProperty, value ); }
		}

		public PaneStates State
		{
			get { return ( PaneStates ) GetValue( StateProperty ); }
			set { SetValue( StateProperty, value ); }
		}

		public object Pane
		{
			get { return GetValue( PaneProperty ); }
			set { SetValue( PaneProperty, value ); }
		}

		public object Content
		{
			get { return GetValue( ContentProperty ); }
			set { SetValue( ContentProperty, value ); }
		}

		public double PanelWidth
		{
			get { return ( double ) GetValue( PanelWidthProperty ); }
			set { SetValue( PanelWidthProperty, value ); }
		}

		public bool EnablePaneSwipe
		{
			get { return ( bool ) GetValue( EnablePaneSwipeProperty ); }
			set
			{
				EnSwipe( value );
				SetValue( EnablePaneSwipeProperty, value );
			}
		}

		public Brush PanelBackground
		{
			get { return ( Brush ) GetValue( PanelBackgroundProperty ); }
			set { SetValue( PanelBackgroundProperty, value ); }
		}

		public PassiveSplitView()
		{
			DefaultStyleKey = typeof( PassiveSplitView );

			// Bind for flow direction changes
			DependencyProperty FlowDir
				= DependencyProperty.Register(
					System.Utils.Md5( "FlowDirBinding" ).Substring( 0, 8 )
					, typeof( FlowDirection )
					, typeof( PassiveSplitView )
					, new PropertyMetadata( FlowDirection.LeftToRight, OnFlowDirectionChanged )
				);
			SetBinding( FlowDir, new Binding() { Path = new PropertyPath( "FlowDirection" ), Source = this } );

			Loaded += PassiveSplitView_Loaded;
		}

		private void PassiveSplitView_Loaded( object sender, RoutedEventArgs e )
		{
			UpdateVisualState( false );
		}

		public void OpenPane()
		{
			State = PaneStates.Opened;
		}

		public void ClosePane()
		{
			State = PaneStates.Closed;
		}
	}
}