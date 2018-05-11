using Windows.UI;

using Net.Astropenguin.DataModel;

namespace GR.Settings.Theme
{

	class ThemeTextBlock : ActiveData
	{
		private ColorItem FGItem;
		public Color Foreground
		{
			get { return FGItem.TColor; }
		}

		private ColorItem BGItem;
		public Color Background
		{
			get { return BGItem.TColor; }
		}

		private ColorItem ShadeItem = new ColorItem( "Transparent", Colors.Transparent );
		public Color Shaded 
		{
			get { return ShadeItem.TColor; }
		}

		public ThemeSet ColorSet { get; private set; }
		public string FGName { get; private set; }
		public string BGName { get; private set; }

		public ThemeTextBlock( string RelativeColor, string ThemeColor, ThemeSet Reference )
		{
			ColorSet = Reference;
			FGName = RelativeColor;
			BGName = ThemeColor;

			DisplayColor();
		}

		public void Shades( int Val )
		{
			ShadeItem = ColorSet.GetColor( "Shades" + Val );
			NotifyChanged( "Shaded" );
		}

		private void DisplayColor()
		{
			FGItem = ColorSet.GetColor( FGName );
			BGItem = ColorSet.GetColor( BGName );

			NotifyChanged( "Foreground", "Background" );
		}
	}
}
