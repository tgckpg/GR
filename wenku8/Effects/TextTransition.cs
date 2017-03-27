using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

using Net.Astropenguin.Helpers;

namespace wenku8.Effects
{
	class TextTransition
	{
		private DispatcherTimer Timer;

		private string From;
		private string To;

		private string RandCharBase = "";

		private const int TickSpeed = 30;

		private Action<string> Animate;
		private Action Complete;

		private float Steps = 20;
		private int charLen;

		// Number of animating characters
		private int AChars = 0;
		// Target Length
		private int TLen = 0;

		private float[] nFactors;

		private int[] Easing;

		public TextTransition( Action<string> CurrentText )
		{
			Timer = new DispatcherTimer();
			Timer.Tick += Timer_Tick;
			Timer.Interval = TimeSpan.FromMilliseconds( TickSpeed );

			Animate = CurrentText;

			SetEasing( 2 );

			char[] split = new char[ 0x7F - 0x21 ];
			for ( int i = 0x21, j = 0; i < 0x7F; i++, j ++ )
			{
				split[ j ] = ( char ) i;
			}
			charLen = split.Length;

			int ranIndex = 0;
			List<int> indexes = new List<int>();
			for ( int i = 0; i < charLen; i++ )
			{
				ranIndex = NTimer.RandInt( 0, charLen );

				if ( !indexes.Contains( ranIndex ) )
				{
					indexes.Add( ranIndex );
				}
				else
				{
					i--;
				}
			}

			foreach ( int value in indexes )
			{
				RandCharBase += split[ value ];
			}
		}

		public void SetTransation( string FromText, string ToText )
		{
			From = FromText;
			To = ToText;
		}

		public void SetDuration( int milliseconds )
		{
			this.Steps = milliseconds;
			SetEasing( 2 );
		}

		public void SetEasing( int power )
		{
			Easing = new int[ ( int ) Steps ];
			for ( int i = 0; i < Steps; i++ )
			{
				Easing[ ( int ) Steps - i - 1 ]
					= ( int ) Math.Round( ( 1 - Math.Pow( i / Steps, power ) ) * Steps );
			}
		}

		public void Play()
		{
			CalculateTextTweening();
			Timer.Start();
		}

		private void CalculateTextTweening()
		{
			AChars = Math.Max( From.Length, To.Length );

			TLen = To.Length;
			nFactors = new float[ AChars ];

			for( int i = 0; i < AChars; i ++ )
			{
				if ( TLen <= i )
				{
					nFactors[ i ] = 3;
				}
				else
				{
					nFactors[ i ] = 1 + ( 1 - i / ( float ) TLen );
				}
			}
		}

		private void Timer_Tick( object sender, object e )
		{
			Worker.UIInvoke(
				() => Animate( GetText( Bt() ) )
			);

			if( Steps <= t )
			{
				Timer.Stop();
				if ( Complete != null ) Worker.UIInvoke( () => Complete() );
			}
		}

		private float t = 0;
		private float Bt()
		{
			return t ++ / Steps;
		}

		public void OnComplete( Action Complete )
		{
			this.Complete = Complete;
		}

		private string d = "";
		private string GetText( float t )
		{
			string od = "";
			for ( int i = 0; i < AChars; i++ )
			{
				float f = nFactors[ i ] * t;
				if ( f < 1 )
				{
					od += GetCharUpdateBy( i, f );
				}
				else
				{
					od += i < TLen ? To[ i ] : ' ';
				}
			}

			return d = od;
		}

		private int CEase = -1;

		private char GetCharUpdateBy( int i, float t )
		{
			// Easing Index
			int Ei = ( int ) Math.Floor( t * ( Steps - 1 ) );
			if ( Easing[ Ei ] == CEase && i < d.Length )
			{
				return d[ i ];
			}
			else
			{
				CEase = Easing[ Ei ];
			}

			if ( t < 0.5 && i < From.Length )
			{
				return From[ i ];
			}

			return ( char ) NTimer.RandInt( 0x21, 0x7E );
		}
	}
}
