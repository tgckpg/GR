using System;
using Windows.UI.Xaml.Data;

namespace wenku8.Converters
{
	using Model.Book;

	sealed public class BookInfoConverter : IValueConverter
	{
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