using System;
using Windows.UI.Xaml.Data;

namespace wenku8.Converters
{
	sealed public class ParallaxConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			double _factor = 0;

			if( parameter is double )
			{
				_factor = ( double ) parameter;
			}
			else
			{
				double.TryParse( ( string ) parameter, out _factor );
			}

			if ( value is double )
			{
				return ( double ) value * _factor;
			}
			return 0;
		}

		public object ConvertBack( object value, Type targetType, object parameter, string language )
		{
			double _factor = 1;
			double.TryParse( ( string ) parameter, out _factor );
			if ( value is double )
			{
				return ( double ) value / ( _factor == 0 ? 1 : _factor );
			}
			return 0;
		}
	}
}