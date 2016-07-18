using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

using Net.Astropenguin.Logging;

namespace wenku8.Converters
{
    using CompositeElement;

    public class SizeVisibilityConverter : IValueConverter
    {
        public static readonly string ID = typeof( SizeVisibilityConverter ).Name;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string mode = ( string ) parameter;

            switch ( ( RectileSize ) value )
            {
                case RectileSize.Large:
                    return ( mode == "CT" ? Visibility.Visible : Visibility.Collapsed );
                case RectileSize.Medium:
                default:
                    return ( mode == "TC" ? Visibility.Visible : Visibility.Collapsed );
            }
        }

        public object ConvertBack( object value, Type targetType, object parameter, string language )
        {
            return Visibility.Collapsed;
        }
    }
}