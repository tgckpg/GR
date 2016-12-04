using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using Net.Astropenguin.Helpers;
using Net.Astropenguin.Logging;

namespace wenku8.CompositeElement
{
    using Effects;
    using Model.ListItem;
    using Resources;

    [TemplatePart( Name = StageName, Type = typeof( CanvasAnimatedControl ) )]
    public sealed class SplashRipple : Control, INotifyPropertyChanged 
    {
        public static readonly string ID = typeof( SplashRipple ).Name;

        #region Property Changed impl
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyChanged( params string[] Names )
        {
            var j = Dispatcher.RunIdleAsync( ( x ) =>
            {
                foreach ( string Name in Names )
                    PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( Name ) );
            } );
        }
        #endregion

        public static readonly DependencyProperty SourceProperty
            = DependencyProperty.Register(
                "Source"
                , typeof( NameValue<string> ), typeof( SplashRipple )
                , new PropertyMetadata( null, OnSourceChanged ) );

        private string _Source;
        public NameValue<string> Source 
        {
            get { return ( NameValue<string> ) GetValue( SourceProperty ); }
            set { SetValue( SourceProperty, value ); }
        }

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            private set
            {
                _IsLoading = value;
                NotifyChanged( "IsLoading" );
            }
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
            Stage.Update += Stage_Update;
        }

        CanvasBitmap bmpImage;
        Vector2 imageSize;

        PixelShaderEffect dissolveEffect;
        PixelShaderEffect rippleEffect;

        private void Stage_CreateResources( CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args )
        {
            args.TrackAsyncAction( CreateResourcesAsync( sender ).AsAsyncAction() );
        }

        async Task CreateResourcesAsync( ICanvasAnimatedControl sender )
        {
            if ( dissolveEffect == null )
            {
                // See Win2D custom effect example
                dissolveEffect = new PixelShaderEffect( await AppStorage.AppXGetBytes( "libwenku8/Shaders/Dissolve.bin" ) );
                rippleEffect = new PixelShaderEffect( await AppStorage.AppXGetBytes( "libwenku8/Shaders/Ripples.bin" ) );
            }

            if ( string.IsNullOrEmpty( _Source ) )
            {
                bmpImage = await CanvasBitmap.LoadAsync( sender, new Uri( "ms-appx:///Assets/Samples/bookcoversample.png" ) );
            }
            else
            {
                Restarted = false;
                using ( Stream s = Shared.Storage.GetStream( _Source ) )
                    bmpImage = await CanvasBitmap.LoadAsync( sender, s.AsRandomAccessStream() );
            }

            imageSize = bmpImage.Size.ToVector2();

            rippleEffect.Properties[ "dpi" ] = sender.Dpi;
            rippleEffect.Properties[ "center" ] = 0.5f * imageSize;

            dissolveEffect.Properties[ "dissolveAmount" ] = 0.5f;

            var j = Dispatcher.RunIdleAsync( ( x ) =>
            {
                Width = imageSize.X;
                Height = imageSize.Y;

                Stage.Margin = new Thickness( 0, -0.5 * imageSize.Y + 150, 0, 0 );
            } );
        }

        private volatile bool Restarted = false;
        private volatile bool ShouldEx = false;
        private volatile float pt = 0;

        private void Stage_Draw( ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args )
        {
            float t = ( float ) args.Timing.TotalTime.TotalMilliseconds;

            if ( pt == 0 ) pt = t;
            t -= pt;

            rippleEffect.Properties[ "t1" ] = Easings.OutQuintic( t, 1500 );
            rippleEffect.Properties[ "t2" ] = Easings.OutQuintic( t, 1900 );

            // Draw the custom effect.
            dissolveEffect.Source1 = bmpImage;
            dissolveEffect.Source2 = rippleEffect;

            args.DrawingSession.DrawImage( dissolveEffect );

            if ( 2000 < t )
            {
                if( ShouldEx )
                {
                    ShouldEx = false;
                    pt = 0;
                    Stage.Draw -= Stage_Draw;
                    Stage.Draw += Stage_DrawEx;
                }
                else
                {
                    sender.Paused = true;
                }
            }
        }

        private async void Stage_DrawEx( ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args )
        {
            float t = ( float ) args.Timing.TotalTime.TotalMilliseconds;

            if ( pt == 0 ) pt = t;
            t -= pt;

            rippleEffect.Properties[ "t1" ] = 1 + Easings.InOutQuintic( t, 1500 );
            rippleEffect.Properties[ "t2" ] = 1 + Easings.InOutQuintic( t, 1900 );

            // Draw the custom effect.
            dissolveEffect.Source1 = bmpImage;
            dissolveEffect.Source2 = rippleEffect;

            args.DrawingSession.DrawImage( dissolveEffect );

            if ( 2000 < t )
            {
                Stage.Draw -= Stage_DrawEx;

                await CreateResourcesAsync( sender );

                pt = 0;
                IsLoading = false;
                Stage.Draw += Stage_Draw;
            }
        }


        private void Stage_Update( ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args )
        {
            if ( Restarted )
            {
                Restarted = false;
                IsLoading = true;
                ShouldEx = true;
            }
        }

        private void Splash()
        {
            Restarted = true;
            _Source = Source.Value;

            if ( Stage != null ) Stage.Paused = false;
        }

        private static void OnSourceChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            ( ( SplashRipple ) d ).Splash();
        }

        public void SplashIn()
        {
            if ( Stage != null ) Stage.Paused = false;
        }

        public void SplashOut()
        {
            Restarted = true;
            if ( Stage != null ) Stage.Paused = false;
        }

    }
}