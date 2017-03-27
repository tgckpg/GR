using System;
using Windows.UI.Xaml.Data;

using Windows.UI.Xaml.Media;

namespace wenku8.Converters
{
	sealed public class PanePosConverter : IValueConverter
	{
		public static readonly string ID = typeof( PanePosConverter ).Name;

		public object Convert( object value, Type targetType, object parameter, string language )
		{
			CompositeTransform Transform = value as CompositeTransform;
			if ( Transform != null )
			{
				bool ModeX = parameter.ToString() == "X";
				return ModeX ? Transform.TranslateX : Transform.TranslateY;
			}

			return Math.Round( ( ( double ) value ) * double.Parse( ( string ) parameter ), 1 );
		}

		public object ConvertBack( object value, Type targetType, object parameter, string language )
		{
			return 0.0;
		}
	}
}