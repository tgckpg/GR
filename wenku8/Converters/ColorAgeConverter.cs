using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace wenku8.Converters
{
    using Effects;
    using Settings.Theme;

    sealed public class ColorAgeConverter : IValueConverter
    {
        public Brush Fill { get; set; }

        public object Convert( object value, Type targetType, object parameter, string language )
        {
            DateTime Time = ( DateTime ) value;

            SolidColorBrush CFill = ( SolidColorBrush ) Fill;
            ColorItem C = new ColorItem( "NaN", CFill.Color );

            C.S = ( int ) ( 100 - Math.Floor( ( DateTime.Now - Time ).TotalDays * 50 ) ).Clamp( 0, 100 );

            return new SolidColorBrush( C.TColor );
        }

        public object ConvertBack( object value, Type targetType, object parameter, string language )
        {
            return value;
        }
    }
}