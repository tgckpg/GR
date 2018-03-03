using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace GR.Converters
{
	using Effects;
	using GR.Model.ListItem;
	using GSystem;
	using Settings.Theme;

	sealed public class ObjectColorConverter : IValueConverter
	{
		public Brush F0 { get; set; }
		public Brush F1 { get; set; }
		public Brush F2 { get; set; }
		public Brush F3 { get; set; }
		public Brush F4 { get; set; }

		public Brush BF { get; set; }

		public Brush[] Brushes => new Brush[] { F0, F1, F2, F3, F4 };

		public object Convert( object value, Type targetType, object parameter, string language )
		{
			TreeItem Item = ( TreeItem ) value;
			Random R = new Random( Utils.Md5Int( Item.ItemTitle ) + 1 );

			SolidColorBrush CFill;
			if ( Item.IsActive )
			{
				CFill = ( SolidColorBrush ) Brushes[ R.Next( Brushes.Length ) ];
			}
			else
			{
				CFill = ( SolidColorBrush ) BF;
			}

			ColorItem C = new ColorItem( "NaN", CFill.Color );

			C.L = C.L - C.L / ( 10 - Item.TreeLevel );
			C.S = Item.IsActive ? 100 : 0;

			return new SolidColorBrush( C.TColor );
		}

		public object ConvertBack( object value, Type targetType, object parameter, string language ) { throw new NotSupportedException(); }
	}
}