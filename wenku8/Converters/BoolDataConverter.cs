using System;
using wenku8.CompositeElement;
using Windows.UI.Xaml.Data;

namespace wenku8.Converters
{
    public class BoolDataConverter : IValueConverter
    {
        public static readonly string ID = typeof( BoolDataConverter ).Name;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null;
        }

        public object ConvertBack( object value, Type targetType, object parameter, string language )
        {
            return false;
        }
    }
}
