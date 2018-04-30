using System;
using Windows.UI.Xaml.Data;

using libtranslate;

namespace GR.Converters
{
	sealed public class CustomPhaseConverter : IValueConverter
	{
		public Translator Conv { get; } = new Translator();

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return Conv.Translate( value as string );
		}

		public object ConvertBack( object value, Type targetType, object parameter, string language )
		{
			return value;
		}
	}
}