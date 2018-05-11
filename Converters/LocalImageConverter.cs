using System;
using Windows.UI.Xaml.Data;

namespace GR.Converters
{
	using Resources;

	sealed public class LocalImageConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, string language )
		{
			if ( value == null ) return value;

			string ImgFile = value.ToString();
			return Shared.Storage.FileExists( ImgFile ) ? "ms-appdata:///local/" + ImgFile : "ms-appx:///Assets/NewContent155x155-200.png";
		}

		public object ConvertBack( object value, Type targetType, object parameter, string language )
		{
			return false;
		}
	}
}