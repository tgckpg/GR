using System;
using Windows.UI.Xaml.Data;

namespace wenku8.Converters
{
    using Model.Book;

    public class BookInfoConverter : IValueConverter
    {
        public static readonly string ID = typeof( BoolDataConverter ).Name;

        public object Convert( object value, Type targetType, object parameter, string language )
        {
            return BookItem.TypeName( ( BookInfo ) value );
        }

        public object ConvertBack( object value, Type targetType, object parameter, string language )
        {
            return false;
        }
    }
}
