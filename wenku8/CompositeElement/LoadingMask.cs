using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Net.Astropenguin.Controls;
using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Messaging;
using Net.Astropenguin.UI;

namespace wenku8.CompositeElement
{
	using Effects;

	[TemplatePart( Name = LoadingRingName, Type = typeof( ProgressRing ))]
	[TemplatePart( Name = TextName, Type = typeof( TextBlock ))]
	public class LoadingMask : StateControl
	{
		public static DependencyProperty TextProperty = DependencyProperty.Register( "Text", typeof( string ), typeof( LoadingMask ), new PropertyMetadata( "Loading" ) );

		public string Text
		{
			get { return ( string ) GetValue( TextProperty ); }
			set { SetValue( TextProperty, value ); }
		}

		new public ControlState State
		{
			get { return base.State; }
			set
			{
				base.State = value;
				Closed = ( value == ControlState.Foreatii );
			}
		}

		protected bool Closed = true;

		private const string LoadingRingName = "LoadingRing";
		private const string TextName = "Message";

		protected ProgressRing LoadingRing;
		protected TextBlock Message;
		protected TextBlock LogText;

		public LoadingMask()
			:base()
		{
			DefaultStyleKey = typeof( LoadingMask );
			MessageBus.OnDelivery += MessageBus_OnDelivery;
		}

		~LoadingMask()
		{
			MessageBus.OnDelivery -= MessageBus_OnDelivery;
		}

		virtual protected void MessageBus_OnDelivery( Message Mesg )
		{
			if ( Message == null || Closed ) return;
			if ( Mesg.TargetType == typeof( System.ActionCenter ) ) return;

			Worker.UIInvoke( () =>
			{
				Message.Text = Mesg.Content;
			} );
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			LoadingRing = GetTemplateChild( LoadingRingName ) as ProgressRing;
			Message = GetTemplateChild( TextName ) as TextBlock;
		}

	}

	[TemplatePart( Name = TipsName, Type = typeof( TextBlock ))]
	public class TipMask : LoadingMask
	{
		private const string TipsName = "Tips";

		// Number of tips
		private const int L = 14;
		private static List<string> EveryMessage;

		private bool Terminate = false;

		protected TextBlock Tips;
		public TipMask()
			:base()
		{
			DefaultStyleKey = typeof( TipMask );

			Loaded += TipMask_Loaded;
		}

		public void Stop()
		{
			Terminate = true;
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			Tips = GetTemplateChild( TipsName ) as TextBlock;
		}

		private void TipMask_Loaded( object sender, RoutedEventArgs e )
		{
			StringResources stx = new StringResources( "Tips" );
			EveryMessage = new List<string>();
			for( int i = 0; i < L; i ++ )
			{
				EveryMessage.Add( stx.Str( ( i + 1 ) + "" ) );
			} 

			LoopMessage();
		}

		private bool Handled = false;
		internal async void HandleBack( Frame F, XBackRequestedEventArgs e )
		{
			if ( e.Handled = Handled ) return;
			e.Handled = Handled = true;
			Terminate = false;
			await MaskOpen();

			await F.Dispatcher.RunIdleAsync( ( x ) =>
			{
				if ( F.CanGoBack ) F.GoBack();
			} );

			MaskClose();
			Terminate = true;
			Handled = false;
		}

		private async Task MaskOpen()
		{
			Closed = false;
			State = ControlState.Reovia;
			await Task.Delay( 1000 );
		}

		private void MaskClose()
		{
			Closed = true;
			State = ControlState.Foreatii;
		}

		internal async void HandleForward( Frame F, Action p )
		{
			await MaskOpen();

			p();

			MaskClose();
		}

		public async void HandleForward( Frame F, Func<Task> p )
		{
			await MaskOpen();

			await p();

			MaskClose();
		}

		protected async void LoopMessage()
		{
			if ( Terminate || Tips == null ) return;

			int i = ( int ) Math.Round( NTimer.RandDouble() * ( L - 1 ) );
			if ( i < EveryMessage.Count )
			{
				Tips.Text = EveryMessage[ i ];
			}

			await Task.Delay( 5000 );
			LoopMessage();
		}

	}

}