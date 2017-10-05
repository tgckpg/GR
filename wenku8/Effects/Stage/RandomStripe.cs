using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;

namespace wenku8.Effects.Stage
{
	using Settings.Theme;

	sealed class RandomStripe
	{
		int MaxTexFactor = 6;
		int MinTexFactor = 1;
		int TexSize = 30;

		public Color[] Palletes;

		private Random R;

		public RandomStripe( int Seed )
		{
			R = new Random( Seed );
			CreateColorScheme();
		}

		private void CreateColorScheme()
		{
			ColorItem C = new ColorItem( "N", Colors.White );
			int OH = R.Next( 0, 360 );

			int l = R.Next( 5, 9 );
			Palletes = new Color[ l ];

			for( int i = 0; i < l;i ++ )
			{
				C.H = ( int ) ( 20.0f * 0.2f * R.NextDouble() ) + OH;
				C.S = R.Next( 50, 101 );
				C.L = R.Next( 10, 66 );

				Palletes[ i ] = C.TColor;
			}

		}

		private Tuple<float, ICanvasBrush, CanvasBlend>[] CreateBrushes( ICanvasResourceCreator Dev )
		{
			Tuple<float, ICanvasBrush, CanvasBlend>[] Brushes = new Tuple<float, ICanvasBrush, CanvasBlend>[ Palletes.Length ];
			for ( int i = 0; i < Palletes.Length; i++ )
			{
				CanvasRenderTarget Target = new CanvasRenderTarget( Dev, TexSize, TexSize, 96 );

				float cap_w = 0;
				CanvasBlend BrushBlend = CanvasBlend.SourceOver;
				using ( CanvasDrawingSession ds = Target.CreateDrawingSession() )
				{
					float xy;
					float s = 0.3333333f * TexSize;
					float r = 0.5f * s * 1.41421354f;
					cap_w = 2 * r;

					int Pn = R.Next( 0, 121 );

					if ( Pn < 10 )
					{
						ds.FillRectangle( s, s, s, s, Palletes[ i ] );
					}
					else if ( 50 <= Pn && Pn < 60 )
					{
						s = TexSize * 0.166666666f;
						ds.FillRectangle( 0, 0.5f * s, TexSize, s, Palletes[ i ] );
						ds.FillRectangle( 0, 2.5f * s, TexSize, s, Palletes[ i ] );
						ds.FillRectangle( 0, 4.5f * s, TexSize, s, Palletes[ i ] );
						BrushBlend = CanvasBlend.Copy;
						cap_w = 0;
					}
					else if ( 80 <= Pn && Pn < 90 )
					{
						xy = 0.5f * TexSize;
						ds.FillCircle( xy, xy, r, Palletes[ i ] );
						ds.FillCircle( 0, 0, r, Palletes[ i ] );
						ds.FillCircle( 0, TexSize, r, Palletes[ i ] );
						ds.FillCircle( TexSize, 0, r, Palletes[ i ] );
						ds.FillCircle( TexSize, TexSize, r, Palletes[ i ] );
					}
					else
					{
						ds.FillRectangle( 0, 0, 30, 30, Palletes[ i ] );
						cap_w = 0;
					}

					// ds.DrawRectangle( 0, 0, TexSize, TexSize, Colors.Black, 0.5f );
				}

				Brushes[ i ] = new Tuple<float, ICanvasBrush, CanvasBlend>( cap_w, new CanvasImageBrush( Dev, Target ) { ExtendX = CanvasEdgeBehavior.Wrap, ExtendY = CanvasEdgeBehavior.Wrap }, BrushBlend );
			}
			return Brushes;
		}

		public CanvasBitmap DrawBitmap( ICanvasResourceCreatorWithDpi Dev, int Width, int Height )
		{
			CanvasRenderTarget RenderTarget = new CanvasRenderTarget( Dev, Width, Height );
			using ( CanvasDrawingSession ds = RenderTarget.CreateDrawingSession() )
			{
				float x0 = 0;
				float x1 = 0;

				bool ColSpace = NTimer.RandChoice( R, true, false );
				bool RowSpace = NTimer.RandChoice( R, true, false );

				Tuple<float, ICanvasBrush, CanvasBlend>[] Brushes = CreateBrushes( Dev );
				float Pi_d4 = 0.7853982f;
				float h = 0.5f * Height;
				float r = h * 1.41421354f;
				float k = r - h;
				float y0 = -k;
				float y1 = Height + k;

				float Thickness, sk;
				while ( x0 < Width || x1 < Width )
				{
					if ( x0 < Width )
					{
						Thickness = R.Next( MinTexFactor, MaxTexFactor + 1 ) * TexSize;
						if ( ColSpace )
						{
							(float cap_w, ICanvasBrush Brush, CanvasBlend BrushBlend) = NTimer.RandChoiceFromList( R, Brushes );
							Brush.Transform = Matrix3x2.CreateTranslation( x0, 0 );

							sk = 0.5f * Thickness;
							ds.Blend = BrushBlend;
							ds.Transform = Matrix3x2.CreateRotation( Pi_d4, new Vector2( x0, h ) );
							ds.DrawLine( x0, y0 - sk, x0, y1 + sk, Brush, Thickness + cap_w );
							x0 += cap_w;
						}
						x0 += Thickness;
						ColSpace = !ColSpace;
					}

					if ( x1 < Width )
					{
						Thickness = R.Next( MinTexFactor, MaxTexFactor + 1 ) * TexSize;
						if ( RowSpace )
						{
							(float cap_w, ICanvasBrush Brush, CanvasBlend BrushBlend) = NTimer.RandChoiceFromList( R, Brushes );
							Brush.Transform = Matrix3x2.CreateTranslation( x1, 0 );

							sk = 0.5f * Thickness;
							ds.Blend = BrushBlend;
							ds.Transform = Matrix3x2.CreateRotation( -Pi_d4, new Vector2( x1, h ) );
							ds.DrawLine( x1, y0 - sk, x1, y1 + sk, Brush, Thickness + cap_w );
							x1 += cap_w;
						}
						x1 += Thickness;
						RowSpace = !RowSpace;
					}
				}
			}

			return RenderTarget;
		}

	}
}