using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Xaml;

namespace GR.Resources
{
	using Config;

	public class LayoutSettings
	{
		public static double ScreenWidth { get { return Window.Current.Bounds.Width; } }
		public static double ScreenHeight { get { return Window.Current.Bounds.Height; } }
		public static uint DisplayWidth { get { return DisplayInformation.GetForCurrentView().ScreenWidthInRawPixels; } }
		public static uint DisplayHeight { get { return DisplayInformation.GetForCurrentView().ScreenHeightInRawPixels; } }

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
			MajorColor = GRConfig.Theme.ColorMajor;
			MinorColor = GRConfig.Theme.ColorMinor;
			MajorBackgroundColor = GRConfig.Theme.BgColorMajor;
			MinorBackgroundColor = GRConfig.Theme.BgColorMinor;
			RelativeMajorBackgroundColor = GRConfig.Theme.RelColorMajorBackground;
			RelativeMajorColor = GRConfig.Theme.RelColorMajor;
			SubtleColor = GRConfig.Theme.SubtleColor;
			VerticalRibbonColor = GRConfig.Theme.RibbonColorVert;
			HorizontalRibbonColor = GRConfig.Theme.RibbonColorHorz;

			Shades10 = GRConfig.Theme.Shades10;
			Shades20 = GRConfig.Theme.Shades20;
			Shades30 = GRConfig.Theme.Shades30;
			Shades40 = GRConfig.Theme.Shades40;
			Shades50 = GRConfig.Theme.Shades50;
			Shades60 = GRConfig.Theme.Shades60;
			Shades70 = GRConfig.Theme.Shades70;
			Shades80 = GRConfig.Theme.Shades80;
			Shades90 = GRConfig.Theme.Shades90;

			RelativeShadesBrush = GRConfig.Theme.RelColorShades;
		}

	}
}