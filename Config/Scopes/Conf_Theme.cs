using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace GR.Config.Scopes
{
	class Conf_Theme : ScopedConfig<Database.Models.Theme>
	{
		public Color RelColorMajorBackground
		{
			get => GetValue( "RelColorMajorBackground", Colors.Black );
			set => SetValue( "RelColorMajorBackground", value );
		}

		public Color RelColorMajor
		{
			get => GetValue( "RelColorMajor", Colors.White );
			set => SetValue( "RelColorMajor", value );
		}

		public Color SubtleColor
		{
			get => GetValue( "SubtleColor", Color.FromArgb( 0xFF, 0x4D, 0x4D, 0x4D ) );
			set => SetValue( "SubtleColor", value );
		}

		public Color BgColorMajor
		{
			get => GetValue( "BgColorMajor", Colors.White );
			set => SetValue( "BgColorMajor", value );
		}

		public Color BgColorMinor
		{
			get => GetValue( "BgColorMinor", Color.FromArgb( 0xE5, 0xE6, 0xE6, 0xE6 ) );
			set => SetValue( "BgColorMinor", value );
		}

		public Color ColorMajor
		{
			get => GetValue( "ColorMajor", Color.FromArgb( 0xDF, 0x3F, 0xA9, 0xF5 ) );
			set => SetValue( "ColorMajor", value );
		}

		public Color ColorMinor
		{
			get => GetValue( "ColorMinor", Color.FromArgb( 0xFF, 0x00, 0x71, 0xBC ) );
			set => SetValue( "ColorMinor", value );
		}

		public Color RibbonColorHorz
		{
			get => GetValue( "RibbonColorHorz", Color.FromArgb( 0xFF, 0x7A, 0xC9, 0x43 ) );
			set => SetValue( "RibbonColorHorz", value );
		}

		public Color RibbonColorVert
		{
			get => GetValue( "RibbonColorVert", Color.FromArgb( 0xFF, 0xDC, 0x14, 0x3C ) );
			set => SetValue( "RibbotColorVert", value );
		}

		public Color Shades10
		{
			get => GetValue( "Shades10", Colors.Black );
			set => SetValue( "Shades10", value );
		}

		public Color Shades20
		{
			get => GetValue( "Shades20", Colors.Black );
			set => SetValue( "Shades20", value );
		}

		public Color Shades30
		{
			get => GetValue( "Shades30", Colors.Black );
			set => SetValue( "Shades30", value );
		}

		public Color Shades40
		{
			get => GetValue( "Shades40", Colors.Black );
			set => SetValue( "Shades40", value );
		}

		public Color Shades50
		{
			get => GetValue( "Shades50", Colors.Black );
			set => SetValue( "Shades50", value );
		}

		public Color Shades60
		{
			get => GetValue( "Shades60", Colors.Black );
			set => SetValue( "Shades60", value );
		}

		public Color Shades70
		{
			get => GetValue( "Shades70", Colors.Black );
			set => SetValue( "Shades70", value );
		}

		public Color Shades80
		{
			get => GetValue( "Shades80", Colors.Black );
			set => SetValue( "Shades80", value );
		}

		public Color Shades90
		{
			get => GetValue( "Shades90", Colors.Black );
			set => SetValue( "Shades90", value );
		}

		public Color RelColorShades
		{
			get => GetValue( "RelColorShades", Color.FromArgb( 0xFF, 0xDF, 0xDF, 0xDF ) );
			set => SetValue( "RelColorShades", value );
		}

	}
}