using Windows.UI;

// wenku8App Static Resources

namespace wenku8.Resources
{
	using Config;
	using Windows.UI.Xaml;

	public class LayoutSettings 
	{
		public static double ScreenWidth { get { return Window.Current.Bounds.Width; } }
		public static double ScreenHeight { get { return Window.Current.Bounds.Height; } }

		public static Color MajorColor { get; private set; }
		public static Color MinorColor { get; private set; }
		public static Color MajorBackgroundColor { get; private set; }
		public static Color MinorBackgroundColor { get; private set; }
		public static Color RelativeMajorColor { get; private set; }
		public static Color RelativeMajorBackgroundColor { get; private set; }
		public static Color SubtleColor { get; private set; }
		public static Color VerticalRibbonColor { get; private set; }
		public static Color HorizontalRibbonColor { get; private set; }

		public static Color Shades10 { get; private set; }
		public static Color Shades20 { get; private set; }
		public static Color Shades30 { get; private set; }
		public static Color Shades40 { get; private set; }
		public static Color Shades50 { get; private set; }
		public static Color Shades60 { get; private set; }
		public static Color Shades70 { get; private set; }
		public static Color Shades80 { get; private set; }
		public static Color Shades90 { get; private set; }

		public static Color RelativeShadesBrush { get; private set; }

		public static bool IsDarkTheme { get; private set; }

		public LayoutSettings()
		{
			// Color Theme Settings
			MajorColor = Properties.APPEARENCE_THEME_MAJOR_COLOR;
			MinorColor = Properties.APPEARENCE_THEME_MINOR_COLOR;
			MajorBackgroundColor = Properties.APPEARENCE_THEME_MAJOR_BACKGROUND_COLOR;
			MinorBackgroundColor = Properties.APPEARENCE_THEME_MINOR_BACKGROUND_COLOR;
			RelativeMajorBackgroundColor = Properties.APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_BACKGROUND;
			RelativeMajorColor = Properties.APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_MAJOR;
			SubtleColor = Properties.APPEARENCE_THEME_SUBTLE_TEXT_COLOR;
			VerticalRibbonColor = Properties.APPEARENCE_THEME_VERTICAL_RIBBON_COLOR;
			HorizontalRibbonColor = Properties.APPEARENCE_THEME_HORIZONTAL_RIBBON_COLOR;

			Shades10 = Properties.APPEARENCE_THEME_SHADES_10;
			Shades20 = Properties.APPEARENCE_THEME_SHADES_20;
			Shades30 = Properties.APPEARENCE_THEME_SHADES_30;
			Shades40 = Properties.APPEARENCE_THEME_SHADES_40;
			Shades50 = Properties.APPEARENCE_THEME_SHADES_50;
			Shades60 = Properties.APPEARENCE_THEME_SHADES_60;
			Shades70 = Properties.APPEARENCE_THEME_SHADES_70;
			Shades80 = Properties.APPEARENCE_THEME_SHADES_80;
			Shades90 = Properties.APPEARENCE_THEME_SHADES_90;

			RelativeShadesBrush = Properties.APPEARENCE_THEME_RELATIVE_SHADES_COLOR;
		}

	}
}
