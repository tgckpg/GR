﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace GR.Config.Scopes
{
	using Resources;

	class Conf_ContentReader : ScopedConfig<Database.Models.ContentReader>
	{
		public Conf_Clock Clock => new Conf_Clock();
		public Conf_EpStepper EpStepper => new Conf_EpStepper();
		public Conf_BgContext BgContext => new Conf_BgContext();
		public Conf_AccelerScroll AccelerScroll => new Conf_AccelerScroll();

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

		public bool IsHorizontal
		{
			get => GetValue<bool>( "IsHorizontal", () => Shared.LocaleDefaults.Get<bool>( "ContentReader.IsHorizontal" ) );
			set => SetValue( "IsHorizontal", value );
		}

		public bool IsRightToLeft
		{
			get => GetValue<bool>( "IsRightToLeft", () => Shared.LocaleDefaults.Get<bool>( "ContentReader.IsRightToLeft" ) );
			set => SetValue( "IsRightToLeft", value );
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

		public class Conf_BgContext : ScopedConfig<Database.Models.ContentReader>, IConf_BgContext
		{
			protected override string ScopeId => "BgContext";

			public string BgType
			{
				get => GetValue<string>( "BgType", null );
				set => SetValue( "BgType", value );
			}

			public string BgValue
			{
				get
				{
					if ( BgType == "System" )
						return "ms-appx:///Assets/Samples/BgContentReader.jpg";
					return GetValue<string>( "BgValue", null );
				}
				set => SetValue( "BgValue", value );
			}
		}

		public class Conf_AccelerScroll : ScopedConfig<Database.Models.ContentReader>
		{
			protected override string ScopeId => "AccelerScroll";

			public bool Asked
			{
				get => GetValue<bool>( "Asked", false );
				set => SetValue( "Asked", value );
			}

			public bool Enable
			{
				get => GetValue<bool>( "Enable", false );
				set => SetValue( "Enable", value );
			}

			public bool TrackAutoAnchor
			{
				get => GetValue<bool>( "TrackAutoAnchor", true );
				set => SetValue( "TrackAutoAnchor", value );
			}

			public float AccelerMultiplier
			{
				get => GetValue<float>( "AccelerMultiplier", 0.5f );
				set => SetValue( "AccelerMultiplier", value );
			}

			public float TerminalVelocity
			{
				get => GetValue<float>( "TerminalVelocity", 30.0f );
				set => SetValue( "TerminalVelocity", value );
			}

			public float BrakeOffset
			{
				get => GetValue<float>( "BrakeOffset", 0.0f );
				set => SetValue( "BrakeOffset", value );
			}

			public float BrakingForce
			{
				get => GetValue<float>( "BrakingForce", 0.3f );
				set => SetValue( "BrakingForce", value );
			}

			public float Brake
			{
				get => GetValue<float>( "Brake", 0.2f );
				set => SetValue( "Brake", value );
			}
		}

	}
}