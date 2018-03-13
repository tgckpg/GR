using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace GR.Config.Scopes
{
	class Theme : ScopedConfig<Database.Models.Theme>
	{
		public Color TextColorRelBackground
		{
			get => GetValue<Color>( "TextColorRelBackground", Colors.Black );
			set => SetValue( "TextColorRelBackground", value );
		}

		public Color TextColorRelMajor
		{
			get => GetValue<Color>( "TextColorRelMajor", Colors.White );
			set => SetValue( "TextColorRelMajor", value );
		}

		public Color TextColorSubtle
		{
			get => GetValue<Color>( "TextColorSubtle", Color.FromArgb( 0xFF, 0x4D, 0x4D, 0x4D ) );
			set => SetValue( "TextColorSubtle", value );
		}

		public Color BgColorMajor
		{
			get => GetValue<Color>( "BgColorMajor", Colors.White );
			set => SetValue( "BgColorMajor", value );
		}

		public Color BgColorMinor
		{
			get => GetValue<Color>( "BgColorMinor", Color.FromArgb( 0xE5, 0xE6, 0xE6, 0xE6 ) );
			set => SetValue( "BgColorMinor", value );
		}

		public Color ColorMajor
		{
			get => GetValue<Color>( "ColorMajor", Color.FromArgb( 0xDF, 0x3F, 0xA9, 0xF5 ) );
			set => SetValue( "ColorMajor", value );
		}

		public Color ColorMinor
		{
			get => GetValue<Color>( "ColorMinor", Color.FromArgb( 0xFF, 0x00, 0x71, 0xBC ) );
			set => SetValue( "ColorMinor", value );
		}

		public Color RibbonColorHorz
		{
			get => GetValue<Color>( "RibbonColorHorz", Color.FromArgb( 0xFF, 0x7A, 0xC9, 0x43 ) );
			set => SetValue( "RibbonColorHorz", value );
		}

		public Color VERTICAL_RIBBON_COLOR
		{
			get => GetValue<Color>( "RibbonColorVert", Color.FromArgb( 0xFF, 0xDC, 0x14, 0x3C ) );
			set => SetValue( "RibbotColorVert", value );
		}
	}
}