using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;

namespace wenku8.CompositeElement
{
    using Effects;
    using Model.Interfaces;
    using Resources;

    [TemplatePart( Name = StageName, Type = typeof( CanvasAnimatedControl ) )]
    public sealed class SplashRipple : Control
    {
        public static readonly string ID = typeof( SplashRipple ).Name;

        public static readonly DependencyProperty UriProperty
            = DependencyProperty.Register(
                "Source", typeof( Uri ), typeof( SplashRipple )
                , new PropertyMetadata( new Uri( "ms-appx:///Assets/Samples/bookcoversample.png" ), OnSourceChanged ) );

        public Uri Source 
        {
            get { return ( Uri ) GetValue( UriProperty ); }
            set { SetValue( UriProperty, value ); }
        }

        private const string StageName = "AnimaControl";

        private CanvasAnimatedControl Stage;

        public SplashRipple()
        {
            DefaultStyleKey = typeof( SplashRipple );
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Stage = ( CanvasAnimatedControl ) GetTemplateChild( StageName );
            Stage.CreateResources += Stage_CreateResources;
            Stage.Draw += Stage_Draw;
        }

        CanvasBitmap bmpImage;
        Vector2 imageSize;

        PixelShaderEffect dissolveEffect;
        PixelShaderEffect rippleEffect;

        private void Stage_CreateResources( CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args )
        {
            args.TrackAsyncAction( CreateResourcesAsync( sender ).AsAsyncAction() );
        }

        async Task CreateResourcesAsync( CanvasAnimatedControl sender )
        {
            bmpImage = await CanvasBitmap.LoadAsync( sender, Source );
            imageSize = bmpImage.Size.ToVector2();

            // See Win2D custom effect example
            dissolveEffect = new PixelShaderEffect( await AppStorage.AppXGetBytes( "libwenku8/Shaders/Dissolve.bin" ) );
            rippleEffect = new PixelShaderEffect( await AppStorage.AppXGetBytes( "libwenku8/Shaders/Ripples.bin" ) );

            rippleEffect.Properties[ "dpi" ] = sender.Dpi;
            rippleEffect.Properties[ "center" ] = 0.5f * imageSize;

            Width = imageSize.X;
            Height = imageSize.Y;
            // Stage.Margin = new Thickness( -0.5 * imageSize.X, -0.5 * imageSize.Y, 0, 0 );

            dissolveEffect.Properties[ "dissolveAmount" ] = 0.5f;
        }

        private volatile bool Restarted = false;
        private volatile float pt = 0;

        private void Stage_Draw( ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args )
        {
            float t = ( float ) args.Timing.TotalTime.TotalMilliseconds;

            if ( Restarted )
            {
                Restarted = false;
                pt = t;
            }

            t -= pt;

            rippleEffect.Properties[ "t1" ] = Easings.OutQuintic( t, 1500 );
            rippleEffect.Properties[ "t2" ] = Easings.OutQuintic( t, 1900 );

            // Draw the custom effect.
            dissolveEffect.Source1 = bmpImage;
            dissolveEffect.Source2 = rippleEffect;

            args.DrawingSession.DrawImage( dissolveEffect );

            if ( 2000 < t ) sender.Paused = true;
        }

        private async void Splash()
        {
            await CreateResourcesAsync( Stage );
            Restarted = true;
            Stage.Paused = false;
        }

        private static void OnSourceChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( SplashRipple ) d ).Splash();
        }

    }
}