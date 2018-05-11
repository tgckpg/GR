using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace GR.Converters
{
	sealed public class TreeMarginConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, string language )
		{
			int level = ( int ) value;
			return new Thickness( 15 * level, 0, 0, 0 );
		}

		public object ConvertBack( object value, Type targetType, object parameter, string language )
		{
			return 0.0;
		}
	}
}