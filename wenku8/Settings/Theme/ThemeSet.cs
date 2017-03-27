using System;
using System.Linq;
using System.Reflection;
using Windows.UI;
using Windows.UI.Xaml;
using System.Collections.Generic;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.IO;

namespace wenku8.Settings.Theme
{
	using Model.Interfaces;
	using System;

	class ThemeSet : ActiveData, INamable
	{
		private static readonly Type Self = typeof( ThemeSet );
		private static Dictionary<int, byte> ShadesStrength = new Dictionary<int, byte> {
			{ 10, 13 }, { 20, 25 }, { 30, 38 }, { 40, 51 }, { 50, 64 }
			, { 60, 76 }, { 70, 89 }, { 80, 102 }, { 90, 115 }
		};

		public bool Editable { get; internal set; }

		public Dictionary<string,Color> ColorDefs;

		public string _name;

		public Visibility IsSystemSet { get { return Editable ? Visibility.Visible : Visibility.Collapsed; } }
		public IEnumerable<ColorItem> Colors
		{
			get
			{
				List<ColorItem> Items = new List<ColorItem>();
				foreach( Color C in ColorDefs.Values )
				{
					Items.Add( new ColorItem( "N", C ) );
				}
				return Items;
			}
		}
		public static Dictionary<string, string> ParamMap = new Dictionary<string, string> {
			{ "a", "APPEARENCE_THEME_MAJOR_COLOR" },
			{ "b", "APPEARENCE_THEME_MINOR_COLOR" },
			{ "c", "APPEARENCE_THEME_MAJOR_BACKGROUND_COLOR" },
			{ "d", "APPEARENCE_THEME_MINOR_BACKGROUND_COLOR" },
			{ "e", "APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_BACKGROUND" },
			{ "f", "APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_MAJOR" },
			{ "g", "APPEARENCE_THEME_SUBTLE_TEXT_COLOR" },
			{ "h", "APPEARENCE_THEME_VERTICAL_RIBBON_COLOR" },
			{ "i", "APPEARENCE_THEME_HORIZONTAL_RIBBON_COLOR" },
			{ "j", "APPEARENCE_THEME_RELATIVE_SHADES_COLOR" },
			{ "k", "APPEARENCE_THEME_SHADES_10" },
			{ "l", "APPEARENCE_THEME_SHADES_20" },
			{ "m", "APPEARENCE_THEME_SHADES_30" },
			{ "n", "APPEARENCE_THEME_SHADES_40" },
			{ "o", "APPEARENCE_THEME_SHADES_50" },
			{ "p", "APPEARENCE_THEME_SHADES_60" },
			{ "q", "APPEARENCE_THEME_SHADES_70" },
			{ "r", "APPEARENCE_THEME_SHADES_80" },
			{ "s", "APPEARENCE_THEME_SHADES_90" },
		};

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				NotifyChanged( "Name" );
			}
		}

		public Color MajorColor 
		{
			get { return ColorDefs[ "a" ]; }
		}
		public Color MinorBackgroundColor
		{
			get { return ColorDefs[ "d" ]; }
		}

		public ThemeSet( string name, bool CanEdit, params Color[] Colors )
		{
			ColorDefs = new Dictionary<string, Color>();

			SetColors( Colors );

			_name = name;
			Editable = CanEdit;
		}

		public void SetColors( Color[] Colors )
		{
			char c = 'a';
			int l = Colors.Length;
			for( int i = 0; i < l; i ++, c ++ )
			{
				ColorDefs[ c.ToString() ] = Colors[ i ];
			}
			NotifyChanged( "Colors", "MajorColor" );
		}

		public void SetColor( ColorItem C )
		{
			string v = C.ColorTag;
			string Key = ParamMap.First( x => x.Value == v ).Key;

			char t = Key[ 0 ];
			if( 'k' <= t && t <= 's' )
			{
				C.A = ShadesStrength[ 10 * ( t - 'k' + 1 ) ];
			}

			ColorDefs[ Key ] = C.TColor;
			NotifyChanged( "Colors", "MinorBackgroundColor" );
		}

		public ColorItem GetColor( string v )
		{
			string Key = ParamMap.First( x => x.Value == v ).Key;
			return new ColorItem( v, ColorDefs[ Key ] );
		}

		public void BlackShades()
		{
			GiveShades( 0, 0, 0 );
		}

		public void GreyShades()
		{
			GiveShades( 24, 24, 24 );
		}

		private void GiveShades( byte R, byte G, byte B )
		{
			int l = ShadesStrength.Count + 1;

			char c = 'k';
			for ( int i = 1; i < l; i ++, c ++ )
			{
				ColorDefs[ c.ToString() ] = new Color()
				{
					A = ShadesStrength[ i * 10 ],
					R = R,
					G = G,
					B = B
				};
			}
		}

		public void Apply()
		{
			Type P = typeof( Config.Properties );
			foreach( KeyValuePair<string, Color> C in ColorDefs )
			{
				PropertyInfo PInfo = P.GetProperty( ParamMap[ C.Key ] );
				PInfo.SetValue( null, C.Value );
			}
		}

		public XKey[] ToXKeys()
		{
			List<XKey> Colors = new List<XKey>();
			foreach( KeyValuePair<string, Color> P in ColorDefs )
			{
				Colors.Add( new XKey( P.Key, ThemeManager.ColorString( P.Value ) ) );
			}

			Colors.Add( Storage.BookStorage.TimeKey );
			Colors.Add( new XKey( AppKeys.LBS_DEL, false ) );
			return Colors.ToArray();
		}


	}

}
