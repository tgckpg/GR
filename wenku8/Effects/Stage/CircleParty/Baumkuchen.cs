using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wenku8.Settings.Theme;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

using Net.Astropenguin.Logging;
using Net.Astropenguin.Linq;

namespace wenku8.Effects.Stage.CircleParty
{
	using Config;
	using Windows.UI.Xaml;
	class Baumkuchen : PartyBase
	{
		public static readonly string ID = typeof( Baumkuchen ).Name;

		private const int RING_COUNT = 8;

		protected Storyboard Mainboard;

		protected Color FromColor;
		protected Color ToColor;

		public Action<int> TouchHandler;

		double SH, SW;

		public Baumkuchen( Canvas Stage )
			: base( Stage )
		{
			this.Stage = Stage;
			SetParty();

			Mainboard.Begin();
			Stage.SizeChanged += Stage_SizeChanged;
			Stage.Unloaded += Stage_Unloaded;
		}

		private void Stage_Unloaded( object sender, RoutedEventArgs e )
		{
			Stage.SizeChanged -= Stage_SizeChanged;
		}

		public void SetParty()
		{
			DefaultEasing = null;

			FromColor = Properties.APPEARENCE_THEME_MAJOR_COLOR;
			FromColor.A = 0x01;
			ToColor = Properties.APPEARENCE_THEME_MAJOR_COLOR;
			ToColor.A = 0xFF;

			Mainboard = new Storyboard();

			SW = Stage.ActualWidth;
			SH = Stage.ActualHeight;

			double MaxDia = Math.Ceiling( Math.Sqrt( SW * SW + SH * SH ) );

			double InnerDia = 200;
			double DiaFactor = ( MaxDia - InnerDia ) / RING_COUNT;

			int l = RING_COUNT + 1;

			for ( int i = 1; i < l; i++ )
			{
				double Dia = InnerDia + i * DiaFactor;

				Ellipse Ring = NewRing( Dia, 0.5 * DiaFactor );

				Ring.RenderTransform = new CompositeTransform()
				{
					TranslateX = 0.5 * ( SW - Dia )
					, TranslateY = 0.5 * ( SH - Dia )
				};

				Stage.Children.Add( Ring );
			}

			Stage.Children.DrawEach(
				( e, i, j ) => SetAnimation( e as Ellipse, 2000 + i * NTimer.RandInt( 5000 ) )
				, NTimer.RandInt
			);

			// Pointer sensitive rings
			for ( int i = 1; i < l; i++ )
			{
				double Dia = InnerDia + i * DiaFactor;

				Ellipse Ring = NewRing( Dia, 0.5 * DiaFactor );
				Ring.Tag = i;

				Ring.RenderTransform = new CompositeTransform()
				{
					TranslateX = 0.5 * ( SW - Dia )
					, TranslateY = 0.5 * ( SH - Dia )
				};

				SetReaction( Ring );

				Stage.Children.Add( Ring );
			}
		}

		private void Stage_SizeChanged( object sender, Windows.UI.Xaml.SizeChangedEventArgs e )
		{
			double NW = Stage.ActualWidth;
			double NH = Stage.ActualHeight;

			foreach ( Ellipse Ellie in Stage.Children.Cast<Ellipse>() )
			{
				CompositeTransform CTrans = ( CompositeTransform ) Ellie.RenderTransform;
				CTrans.TranslateX = 0.5 * ( NW - Ellie.Width );
				CTrans.TranslateY = 0.5 * ( NH - Ellie.Height );
			}
		}

		public void Flush()
		{
			Mainboard.Stop();
			while ( Stage.Children.ElementAtOrDefault( RING_COUNT ) != null )
			{
				Stage.Children.RemoveAt( RING_COUNT );
			}
		}

		private Ellipse NewRing( double OuterDia, double Bandwidth )
		{
			OuterDia = Math.Ceiling( OuterDia );
			Bandwidth = Math.Ceiling( Bandwidth );

			Ellipse Ellie = new Ellipse() { Width = OuterDia, Height = OuterDia };
			Ellie.StrokeThickness = Bandwidth;

			return Ellie;
		}

		protected void SetReaction( Ellipse Ellie )
		{
			// Enter
			Storyboard Hoverboard = new Storyboard();

			Color C1 = FromColor;
			Color C2 = ToColor;

			ColorAnimationUsingKeyFrames d2 = CreateKeyFrames( C1, C2, 0, 200 );

			SolidColorBrush Brushy = new SolidColorBrush();
			Ellie.Stroke = Brushy;

			Storyboard.SetTarget( d2, Brushy );
			Storyboard.SetTargetProperty( d2, "Color" );

			Hoverboard.Children.Add( d2 );

			Ellie.PointerEntered += ( s, e ) =>
			{
				Hoverboard.Begin();
				TouchHandler?.Invoke( ( int ) ( ( Ellipse ) s ).Tag );
			};

			// Exit
			Storyboard Lostboard = new Storyboard();

			ColorAnimationUsingKeyFrames d1 = CreateKeyFrames( C2, C1, 0, 200 );

			Storyboard.SetTarget( d1, Brushy );
			Storyboard.SetTargetProperty( d1, "Color" );

			Lostboard.Children.Add( d1 );

			Ellie.PointerExited += ( s, e ) =>
			{
				Lostboard.Begin();
			};
		}

		protected void SetAnimation( Ellipse Ellie, int DelayInMilli )
		{
			int standfor = DelayInMilli;

			Color C1 = FromColor;
			Color C2 = ToColor;
			C2.A = 0x66;

			ColorAnimationUsingKeyFrames d2 = CreateKeyFrames( C1, C2, standfor, 200 );

			CreateKeyFrames( C2, C1, 0, 1000 + NTimer.RandInt( 200, 3000 ), standfor + 200, d2 );

			SolidColorBrush Brushy = new SolidColorBrush();
			Ellie.Stroke = Brushy;

			Storyboard.SetTarget( d2, Brushy );
			Storyboard.SetTargetProperty( d2, "Color" );

			Mainboard.Children.Add( d2 );
		}

	}
}
