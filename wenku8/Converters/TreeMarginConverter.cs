using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

using Net.Astropenguin.Logging;

namespace wenku8.Converters
{
    public class TreeMarginConverter : IValueConverter
    {
        public static readonly string ID = typeof( TreeMarginConverter ).Name;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int level = ( int ) value;
            return new Thickness( 20 * level, 0, 0, 0 );
        }

        public object ConvertBack( object value, Type targetType, object parameter, string language )
        {
            return 0.0;
        }
    }
}
