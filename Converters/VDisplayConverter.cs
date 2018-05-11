using System;
using Windows.UI.Xaml.Data;

namespace GR.Converters
{
	using Resources;
	sealed public class VDisplayConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return Shared.Conv.VText.Translate( value as string );
		}

		public object ConvertBack( object value, Type targetType, object parameter, string language )
		{
			return value;
		}
	}
}