using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace GR.Config.Scopes
{
	using Resources;

	class ContentReader : ScopedConfig<Database.Models.ContentReader>
	{
		public Conf_Clock Clock => new Conf_Clock();
		public Conf_EpStepper EpStepper => new Conf_EpStepper();

		public bool UseInertia
		{
			get => GetValue<bool>( "UseInertia", () => Shared.LocaleDefaults.Get<bool>( "ContentReader.UseInertia" ) );
			set => SetValue( "UseInertia", value );
		}

		public bool AutoBookmark
		{
			get => GetValue<bool>( "AutoBookmark", true );
			set => SetValue( "AutoBookmark", value );
		}

		public bool ReadingAnchor
		{
			get => GetValue<bool>( "ReadingAnchor", true );
			set => SetValue( "ReadingAnchor", value );
		}

		public bool DoubleTap
		{
			get => GetValue<bool>( "DoubleTap", false );
			set => SetValue( "DoubleTap", value );
		}

		public bool EmbedIllus
		{
			get => GetValue<bool>( "EmbedIllus", false );
			set => SetValue( "EmbedIllus", value );
		}

		public bool LeftContext
		{
			get => GetValue<bool>( "LeftContext", true );
			set => SetValue( "LeftContext", value );
		}

		public Windows.UI.Text.FontWeight FontWeight
		{
			get => new Windows.UI.Text.FontWeight() { Weight = GetValue<ushort>( "FontWeight", Windows.UI.Text.FontWeights.Normal.Weight ) };
			set => SetValue( "FontWeight", value.Weight );
		}

		public double FontSize
		{
			get => GetValue<double>( "FontSize", 20.0 );
			set => SetValue( "FontSize", value );
		}

		public double LineHeight
		{
			get => GetValue<double>( "LineHeight", 5.0 );
			set => SetValue( "LineHeight", value );
		}

		public double BlockHeight
		{
			get => GetValue<double>( "BlockHeight", 21.7 );
			set => SetValue( "BlockHeight", value );
		}

		public double ParagraphSpacing
		{
			get => GetValue<double>( "ParagraphSpacing", 32.0 );
			set => SetValue( "ParagraphSpacing", 0.5 * value );
		}

		public Color BackgroundColor
		{
			get => GetValue<Color>( "BackgroundColor", Colors.White );
			set => SetValue( "BackgroundColor", value );
		}

		public Color ScrollBarColor
		{
			get => GetValue<Color>( "ScrollBarColor", Color.FromArgb( 0x3C, 0xFF, 0xFF, 0xFF ) );
			set => SetValue( "ScrollBarColor", value );
		}

		public Color BgColorAssist
		{
			get => GetValue<Color>( "BgColorAssist", Colors.Black );
			set => SetValue( "BgColorAssist", value );
		}

		public Color BgColorNav
		{
			get => GetValue<Color>( "BgColorNav", () => GRConfig.Theme.ColorMajor );
			set => SetValue( "BgColorNav", value );
		}

		public Color FontColor
		{
			get => GetValue<Color>( "FontColor", Color.FromArgb( 0xE5, 0, 0, 0 ) );
			set => SetValue( "FontColor", value );
		}

		public Color TapBrushColor
		{
			get => GetValue<Color>( "TapBrushColor", Color.FromArgb( 0xFF, 0x3F, 0xA9, 0xF5 ) );
			set => SetValue( "TapBrushColor", value );
		}

		public class Conf_Clock : ScopedConfig<Database.Models.ContentReader>
		{
			protected override string ScopeId => "Clock";

			public Color ARColor
			{
				get => GetValue<Color>( "ARColor", Color.FromArgb( 0x50, 0xFF, 0xFF, 0xFF ) );
				set => SetValue( "HHColor", value );
			}

			public Color HHColor
			{
				get => GetValue<Color>( "HHColor", Color.FromArgb( 0x78, 0xEE, 0xEB, 0xAA ) );
				set => SetValue( "HHColor", value );
			}

			public Color MHColor
			{
				get => GetValue<Color>( "MHColor", Color.FromArgb( 0x50, 0xFF, 0xFF, 0xFF ) );
				set => SetValue( "MHColor", value );
			}

			public Color SColor
			{
				get => GetValue<Color>( "SColor", Color.FromArgb( 0x3C, 0xFF, 0xFF, 0xFF ) );
				set => SetValue( "SColor", value );
			}
		}

		public class Conf_EpStepper : ScopedConfig<Database.Models.ContentReader>
		{
			protected override string ScopeId => "EpStepper";

			public Color SColor
			{
				get => GetValue<Color>( "SColor", Color.FromArgb( 0xFF, 0xFF, 0xFF, 0xFF ) );
				set => SetValue( "SColor", value );
			}

			public Color DColor
			{
				get => GetValue<Color>( "DColor", Color.FromArgb( 0xC7, 0xFF, 0xFF, 0xFF ) );
				set => SetValue( "DColor", value );
			}

			public Color BackgroundColor
			{
				get => GetValue<Color>( "BackgroundColor", Color.FromArgb( 0x70, 0x00, 0x00, 0x00 ) );
				set => SetValue( "BackgroundColor", value );
			}
		}

	}
}