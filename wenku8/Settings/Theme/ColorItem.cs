using System;
using Windows.UI;

using Net.Astropenguin.DataModel;

namespace wenku8.Settings.Theme
{
    class ColorItem : ActiveData
    {
        public Color TColor { get { return OColor; } }
        public string ColorTag { get; private set; }

        public Action<Color> BindAction { get; set; }

        private Color OColor;

        private int CMYK_C = 0;
        private int CMYK_M = 0;
        private int CMYK_Y = 0;
        private int CMYK_K = 0;

        private int HSL_H = 0;
        private int HSL_S = 0;
        private int HSL_L = 0;

        #region RGB
        public int R
        {
            get { return TColor.R; }
            set {
                OColor.R = ( byte ) value; SetFromRGB();
            }
        }
        public int G
        {
            get { return TColor.G; }
            set {
                OColor.G = ( byte ) value; SetFromRGB();
            }
        }
        public int B
        {
            get { return TColor.B; }
            set {
                OColor.B = ( byte ) value; SetFromRGB();
            }
        }
        #endregion

        #region CMYK
        public int C
        {
            get { return CMYK_C; }
            set { CMYK_C = value; SetFromCMYK(); }
        }
        public int M
        {
            get { return CMYK_M; }
            set { CMYK_M = value; SetFromCMYK(); }
        }
        public int Y
        {
            get { return CMYK_Y; }
            set { CMYK_Y = value; SetFromCMYK(); }
        }
        public int K
        {
            get { return CMYK_K; }
            set { CMYK_K = value; SetFromCMYK(); }
        }
        #endregion

        #region HSL
        public int H
        {
            get { return HSL_H; }
            set
            {
                HSL_H = value;
                if( 360 < HSL_H ) HSL_H -= 360;
                SetFromHSL();
            }
        }
        public int S
        {
            get { return HSL_S; }
            set { HSL_S = value; SetFromHSL(); }
        }
        public int L
        {
            get { return HSL_L; }
            set { HSL_L = value; SetFromHSL(); }
        }
        #endregion

        public int A
        {
            get { return TColor.A; }
            set {
                OColor.A = ( byte ) value;
                NotifyChanged( "A", "Opacity", "Hex", "TColor" );
            }
        }
        public int Opacity
        {
            get { return ( int ) Math.Floor( TColor.A / 255.0 * 100.0 ); }
            set
            {
                OColor.A = ( byte ) ( int ) Math.Floor( value / 100.0 * 255 );
                NotifyChanged( "A", "Opacity", "Hex", "TColor" );
            }
        }

        public string Hex
        {
            get { return System.ThemeManager.ColorString( TColor ); }
            set
            {
                OColor = System.ThemeManager.StringColor( value );
            }
        }

		public ColorItem( string name, Color color )
		{
			ColorTag = name;
			OColor = color;
            SetCMYK(); SetHSL();
		}

		public void ChangeColor( Color c )
		{
			OColor = c;
            SetCMYK(); SetHSL();

			NotifyChanged(
                "R", "G", "B"
                , "H", "S", "L"
                , "C", "M", "Y", "K"
                , "A", "Opacity"
                , "Hex", "TColor"
            );

            if( BindAction != null ) BindAction( c );
		}

        private void SetCMYK()
        {
            decimal C = 0;
            decimal M = 0;
            decimal Y = 0;

            CMYK_K = 0;

            decimal R = TColor.R;
            decimal G = TColor.G;
            decimal B = TColor.B;

            // Black
            if ( R == 0 && G == 0 && B == 0 )
            {
                CMYK_C = CMYK_M = CMYK_Y = 0;
                CMYK_K = 100;
                return;
            }

            C = 1 - ( R / 255 );
            M = 1 - ( G / 255 );
            Y = 1 - ( B / 255 );

            decimal minCMY = Math.Min( C, Math.Min( M, Y ) );

            CMYK_C = ( int ) Math.Floor( ( C - minCMY ) / ( 1 - minCMY ) * 100 );
            CMYK_M = ( int ) Math.Floor( ( M - minCMY ) / ( 1 - minCMY ) * 100 );
            CMYK_Y = ( int ) Math.Floor( ( Y - minCMY ) / ( 1 - minCMY ) * 100 );
            CMYK_K = ( int ) Math.Floor( minCMY * 100 );
        }

        private void SetHSL()
        {
            Color C = TColor;
            double R = C.R / 255.0;
            double G = C.G / 255.0;
            double B = C.B / 255.0;
            double MV;
            double r2, g2, b2;

            double H = 0;
            double S = 0;
            double L = 0;

            if ( R == G && G == B )
            {
                if ( R == 1 )
                {
                    HSL_L = 100;
                    return;
                }
                else if( R == 0 )
                {
                    HSL_L = 0;
                    return;
                }
            }

            double V = Math.Max( R, G );
            V = Math.Max( V, B );

            double M = Math.Min( R, G );
            M = Math.Min( M, B );

            L = ( M + V ) / 2.0;

            if ( L <= 0.0 )
            {
                return;
            }

            MV = V - M;
            S = MV;
            if ( S > 0.0 )
            {
                S /= ( L <= 0.5 ) ? ( V + M ) : ( 2.0 - V - M );
                r2 = ( V - R ) / MV;
                g2 = ( V - G ) / MV;
                b2 = ( V - B ) / MV;

                if ( R == V )
                {
                    H = ( G == M ? 5.0 + b2 : 1.0 - g2 );
                }
                else if ( G == V )
                {
                    H = ( B == M ? 1.0 + r2 : 3.0 - b2 );
                }
                else
                {
                    H = ( R == M ? 3.0 + g2 : 5.0 - r2 );
                }
                H /= 6.0;
            }

            HSL_H = ( int ) Math.Floor( H * 360 );
            HSL_S = ( int ) Math.Floor( S * 100 );
            HSL_L = ( int ) Math.Floor( L * 100 );
        }

        private void SetFromRGB()
        {
            SetCMYK();
            SetHSL();
            NotifyChanged( "C", "M", "Y", "K", "H", "S", "L", "Hex", "TColor" );
        }

        private void SetFromCMYK()
        {
            OColor.R = ( byte ) ( int ) ( 255 * ( 100 - CMYK_C ) * ( 100 - CMYK_K ) );
            OColor.G = ( byte ) ( int ) ( 255 * ( 100 - CMYK_M ) * ( 100 - CMYK_K ) );
            OColor.B = ( byte ) ( int ) ( 255 * ( 100 - CMYK_Y ) * ( 100 - CMYK_K ) );
            NotifyChanged( "R", "G", "B", "H", "S", "L", "Hex", "TColor" );
        }

        private void SetFromHSL()
        {
            double H = HSL_H / 360.0;
            double S = HSL_S / 100.0;
            double L = HSL_L / 100.0;

            double R = OColor.R / 255.0;
            double G = OColor.G / 255.0;
            double B = OColor.B / 255.0;

            double V = ( L <= 0.5 )
                ? ( L * ( 1.0 + S ) )
                : ( L + S - L * S )
                ;

            if ( V > 0 )
            {
                double m = 2 * L - V;
                double sv = ( V - m ) / V;

                H *= 6.0;

                int Sextent = ( int ) H;
                double fRact = H - Sextent;
                double vsf = V * sv * fRact;
                double mid1 = m + vsf;
                double mid2 = V - vsf;

                switch ( Sextent )
                {
                    case 0:
                    case 6:
                        R = V;
                        G = mid1;
                        B = m;
                        break;

                    case 1:
                        R = mid2;
                        G = V;
                        B = m;
                        break;

                    case 2:
                        R = m;
                        G = V;
                        B = mid1;
                        break;

                    case 3:
                        R = m;
                        G = mid2;
                        B = V;
                        break;

                    case 4:
                        R = mid1;
                        G = m;
                        B = V;
                        break;

                    case 5:
                        R = V;
                        G = m;
                        B = mid2;
                        break;
                }
            }

            OColor.R = Convert.ToByte( R * 255.0f );
            OColor.G = Convert.ToByte( G * 255.0f );
            OColor.B = Convert.ToByte( B * 255.0f );
            NotifyChanged( "R", "G", "B", "C", "M", "Y", "K", "Hex", "TColor" );
        }
    }

}
