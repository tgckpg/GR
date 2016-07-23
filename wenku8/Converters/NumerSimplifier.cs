using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Data;

using Net.Astropenguin.Loaders;

namespace wenku8.Converters
{
    public sealed class NumberSimplifier : IValueConverter
    {
        private static Regex Number = new Regex( @"(\d[\d,]+)" );

        public object Convert( object value, Type targetType, object parameter, string language )
        {
            string abbrd = value.ToString();
            Match m = Number.Match( abbrd );

            if ( m.Success )
            {
                string Size = m.Groups[ 1 ].Value.Replace( ",", "" );

                StringResources stx = new StringResources( "Numbers" );
                string Sim = "";

                int l = 24;

                string Rounded = Size;

                int g = 1;

                int sl = Size.Length;
                if ( 3 < sl )
                {
                    for ( int i = 3; i <= l; i++ )
                    {
                        if ( i < sl )
                        {
                            Sim = stx.Str( "e" + i );
                            g = string.IsNullOrEmpty( Sim ) ? g + 1 : 1;
                        }
                        else
                        {
                            i -= g;
                            Sim = stx.Str( "e" + i );

                            Rounded = Size.Substring( 0, sl - i ) + "." + Math.Round( int.Parse( Size.Substring( sl - i, 2 ) ) / 10.0 );

                            // If it is the type of 1234.5
                            // Trim this thing to 4 digits
                            if( Rounded.Length == 6 )
                            {
                                Rounded = Math.Round( double.Parse( Rounded ) ) + "";
                            }
                            break;
                        }
                    }
                }

                if ( !string.IsNullOrEmpty( Sim ) )
                {
                    return abbrd.Replace( m.Groups[ 1 ].Value, Rounded + " " + Sim + " " );
                }
            }

            return abbrd;
        }

        public object ConvertBack( object value, Type targetType, object parameter, string language )
        {
            return false;
        }
    }
}