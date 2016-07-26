using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

using Net.Astropenguin.IO;
using Net.Astropenguin.Loaders;

namespace wenku8.System
{
    using Settings;
    using Settings.Theme;

    sealed class ThemeManager
    {
        public XRegistry WThemeReg;

        public static ThemeSet DefaultDark()
        {
            StringResources stx = new StringResources( "Settings" );
            return new ThemeSet(
                stx.Text( "Appearance_Theme_Dark" ) + " ( " + stx.Text( "Appearance_Theme_System" ) + " ) "
                , false
                , Color.FromArgb( 0xFF, 0xDD, 0xA0, 0xDD )
                , Color.FromArgb( 0xFF, 0xDA, 0x70, 0xD6 )
                , Colors.Black
                , Color.FromArgb( 0xD0, 0x1A, 0x1A, 0x1A )
                , Colors.White
                , Colors.White
                , Color.FromArgb( 0xFF, 0xB3, 0xB3, 0xB3 )
                , Color.FromArgb( 0xFF, 0x7A, 0xC9, 0x43 )
                , Color.FromArgb( 0xFF, 0x66, 0x2D, 0x91 )
                , Color.FromArgb( 0xFF, 0xEA, 0xEA, 0xEA )
            );
        }

        public static ThemeSet DefaultLight()
        {
            StringResources stx = new StringResources( "Settings" );
            return new ThemeSet(
                stx.Text( "Appearance_Theme_Light" ) + " ( " + stx.Text( "Appearance_Theme_System" ) + ", " + stx.Text( "Appearance_Theme_Default" ) + " ) "
                , false
                , Color.FromArgb( 0xFF, 0x3F, 0xA9, 0xF5 )
                , Color.FromArgb( 0xFF, 0x00, 0x71, 0xBC )
                , Colors.White
                , Color.FromArgb( 0xD0, 0xE6, 0xE6, 0xE6 )
                , Colors.Black
                , Colors.White
                , Color.FromArgb( 0xFF, 0x4D, 0x4D, 0x4D )
                , Color.FromArgb( 0xFF, 0xDC, 0x14, 0x3C )
                , Color.FromArgb( 0xFF, 0x7A, 0xC9, 0x43 )
                , Colors.White
            );
        }

		private const string TFileName = FileLinks.ROOT_SETTING + FileLinks.THEME_SET;

		public ThemeManager()
		{
            WThemeReg = new XRegistry( AppKeys.TS_CXML, TFileName );
		}

        public void Remove( string Name )
        {
			WThemeReg.RemoveParameter( Name );
            Save();
        }

		public void Save( ThemeSet ColorSet )
		{
			WThemeReg.SetParameter( ColorSet.Name, ColorSet.ToXKeys() );
			Save();
		}

		private void Save()
		{
			WThemeReg.Save();
		}

		public ThemeSet CopyTheme( ThemeSet ColorSet, string TargetName )
		{
			WThemeReg.SetParameter( TargetName, ColorSet.ToXKeys() );
			Save();
			XParameter p = WThemeReg.Parameter( TargetName );
			return new ThemeSet( TargetName
				, true
				, ColorSet.ColorDefs[ "a" ], ColorSet.ColorDefs[ "b" ]
				, ColorSet.ColorDefs[ "c" ], ColorSet.ColorDefs[ "d" ]
				, ColorSet.ColorDefs[ "e" ], ColorSet.ColorDefs[ "f" ]
				, ColorSet.ColorDefs[ "g" ], ColorSet.ColorDefs[ "h" ]
				, ColorSet.ColorDefs[ "i" ], ColorSet.ColorDefs[ "j" ]
				, ColorSet.ColorDefs[ "k" ], ColorSet.ColorDefs[ "l" ]
				, ColorSet.ColorDefs[ "m" ], ColorSet.ColorDefs[ "n" ]
				, ColorSet.ColorDefs[ "o" ], ColorSet.ColorDefs[ "p" ]
				, ColorSet.ColorDefs[ "q" ], ColorSet.ColorDefs[ "r" ]
				, ColorSet.ColorDefs[ "s" ]
            );
		}

        public void RemoveTheme( string ThemeName )
		{
			WThemeReg.RemoveParameter( ThemeName );
			Save();
		}

		public bool TestName( string Name )
		{
			return ( WThemeReg.Parameter( Name ) != null );
		}

        public  ThemeSet[] GetThemes()
        {
            XParameter[] p = WThemeReg.Parameters();

            int l = p.Count();
            ThemeSet[] t = new ThemeSet[ l + 2 ];
            t[ 0 ] = DefaultDark();
            t[ 1 ] = DefaultLight();
            t[ 0 ].GreyShades();
            t[ 1 ].BlackShades();

            for ( int i = 0; i < l; i++ )
            {
                t[ i + 2 ] = new ThemeSet(
                    p[ i ].Id, true
                    , StringColor( p[ i ].GetValue( "a" ) ), StringColor( p[ i ].GetValue( "b" ) )
                    , StringColor( p[ i ].GetValue( "c" ) ), StringColor( p[ i ].GetValue( "d" ) )
                    , StringColor( p[ i ].GetValue( "e" ) ), StringColor( p[ i ].GetValue( "f" ) )
                    , StringColor( p[ i ].GetValue( "g" ) ), StringColor( p[ i ].GetValue( "h" ) )
                    , StringColor( p[ i ].GetValue( "i" ) ), StringColor( p[ i ].GetValue( "j" ) )
                    , StringColor( p[ i ].GetValue( "k" ) ), StringColor( p[ i ].GetValue( "l" ) )
                    , StringColor( p[ i ].GetValue( "m" ) ), StringColor( p[ i ].GetValue( "n" ) )
                    , StringColor( p[ i ].GetValue( "o" ) ), StringColor( p[ i ].GetValue( "p" ) )
                    , StringColor( p[ i ].GetValue( "q" ) ), StringColor( p[ i ].GetValue( "r" ) )
                    , StringColor( p[ i ].GetValue( "s" ) )
                );
            }
            return t;
        }

		public static Color StringColor( string p )
		{
			byte a = Convert.ToByte( p.Substring( 1, 2 ) , 16 );
			byte r = Convert.ToByte( p.Substring( 3, 2 ) , 16 );
			byte g = Convert.ToByte( p.Substring( 5, 2 ) , 16 );
			byte b = Convert.ToByte( p.Substring( 7, 2 ) , 16 );
			return Color.FromArgb( a, r, g, b );
		}

		public static string ColorString( Color C )
		{
            return string.Format( "#{0:X2}{1:X2}{2:X2}{3:X2}", C.A, C.R, C.G, C.B );
		}

        public static IEnumerable<ColorItem> PresetColors()
        {
            List<ColorItem> PresetColors = new List<ColorItem>();

            IEnumerable<PropertyInfo> Colors = typeof( Colors ).GetRuntimeProperties();
            Type TypeColor = typeof( Color );
            foreach( PropertyInfo Info in Colors )
            {
                if( Info.PropertyType == TypeColor )
                {
                    PresetColors.Add( new ColorItem( Info.Name, ( Color ) Info.GetValue( Colors ) ) );
                }
            }
            return PresetColors;
        }

		public void Finallize()
		{
			WThemeReg = null;
		}

        public async Task OneDriveSync()
        {
            await Storage.OneDriveSync.Instance.SyncRegistry( WThemeReg, Storage.OneDriveSync.SyncMode.AUTO );
        }
	}
}
