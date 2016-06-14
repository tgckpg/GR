using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace wenku8.Converters
{
    public class FavConverter : IValueConverter
    {
        public static readonly string ID = typeof( FavConverter ).Name;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool IsFaved = ( bool ) value;

            // ToggleFav
            bool ToggleFav = parameter.ToString() == "1";

            if ( ToggleFav )
            {
                return IsFaved ? Visibility.Visible : Visibility.Collapsed;
            }

            return IsFaved ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack( object value, Type targetType, object parameter, string language )
        {
            return false;
        }
    }
}
