using System;
using Windows.UI.Xaml.Data;

using libtranslate;

namespace wenku8.Converters
{
    public class VDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Symbolic.ToVertical( value as string );
        }

        public object ConvertBack( object value, Type targetType, object parameter, string language )
        {
            return value;
        }
    }
}
