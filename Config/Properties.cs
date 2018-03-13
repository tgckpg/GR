using System;
using Windows.UI;

namespace GR.Config
{
	sealed class Properties: AppSettings
	{
		public static bool FIRST_TIME_RUN
		{
			get
			{
				return GetValue<bool>( Parameters.FIRST_TIME_RUN );
			}
			set
			{
				SetParameter( Parameters.FIRST_TIME_RUN, value );
			}
		}

		public static bool RESTORE_MODE
		{
			get
			{
				return GetValue<bool>( Parameters.RESTORE_MODE );
			}
			set
			{
				SetParameter( Parameters.RESTORE_MODE, value );
			}
		}

		public static string INSTALLATION_INST
		{
			get
			{
				return GetValue<string>( Parameters.INSTALLATION_INST );
			}
			set
			{
				SetParameter( Parameters.INSTALLATION_INST, value );
			}
		}

		public static string VERSION
		{
			get
			{
				return GetValue<string>( Parameters.VERSION );
			}
			set
			{
				SetParameter( Parameters.VERSION, value );
			}
		}

		#region Logging
		public static bool ENABLE_SYSTEM_LOG
		{
			get
			{
				return GetValue<bool>( Parameters.ENABLE_SYSTEM_LOG );
			}
			set
			{
				SetParameter( Parameters.ENABLE_SYSTEM_LOG, value );
			}
		}

		public static bool ENABLE_RSYSTEM_LOG
		{
			get
			{
				return GetValue<bool>( Parameters.ENABLE_RSYSTEM_LOG );
			}
			set
			{
				SetParameter( Parameters.ENABLE_RSYSTEM_LOG, value );
			}
		}

		public static string LOG_LEVEL 
		{
			get
			{
				return GetValue<string>( Parameters.LOG_LEVEL );
			}
			set
			{
				SetParameter( Parameters.LOG_LEVEL, value );
			}
		}

		public static string RSYSTEM_LOG_ADDRESS
		{
			get
			{
				return GetValue<string>( Parameters.RSYSTEM_LOG_ADDRESS );
			}
			set
			{
				SetParameter( Parameters.RSYSTEM_LOG_ADDRESS, value );
			}
		}
		#endregion

		#region Appearance

		#region Theme
		public static Color APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_BACKGROUND
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_BACKGROUND ) )
				{
					return SetColor( Parameters.APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_BACKGROUND, Colors.Black );
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_BACKGROUND ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_BACKGROUND, value );
			}
		}
		public static Color APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_MAJOR
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_MAJOR ) )
				{
					return SetColor( Parameters.APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_MAJOR, Colors.White );
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_MAJOR ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_MAJOR, value );
			}
		}
		public static Color APPEARENCE_THEME_SUBTLE_TEXT_COLOR
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_SUBTLE_TEXT_COLOR ) )
				{
					return SetColor( Parameters.APPEARENCE_THEME_SUBTLE_TEXT_COLOR, Color.FromArgb( 0xFF, 0x4D, 0x4D, 0x4D ) );
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_SUBTLE_TEXT_COLOR ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_SUBTLE_TEXT_COLOR, value );
			}
		}
		public static Color APPEARENCE_THEME_MAJOR_BACKGROUND_COLOR
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_MAJOR_BACKGROUND_COLOR ) )
				{
					return SetColor( Parameters.APPEARENCE_THEME_MAJOR_BACKGROUND_COLOR, Colors.White );
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_MAJOR_BACKGROUND_COLOR ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_MAJOR_BACKGROUND_COLOR, value );
			}
		}
		public static Color APPEARENCE_THEME_MINOR_BACKGROUND_COLOR
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_MINOR_BACKGROUND_COLOR ) )
				{
					return SetColor( Parameters.APPEARENCE_THEME_MINOR_BACKGROUND_COLOR, Color.FromArgb( 0xE5, 0xE6, 0xE6, 0xE6 ) );
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_MINOR_BACKGROUND_COLOR ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_MINOR_BACKGROUND_COLOR, value );
			}
		}
		public static Color APPEARENCE_THEME_MAJOR_COLOR
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_MAJOR_COLOR ) )
				{
					return SetColor( Parameters.APPEARENCE_THEME_MAJOR_COLOR, Color.FromArgb( 0xDF, 0x3F, 0xA9, 0xF5 ) );
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_MAJOR_COLOR ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_MAJOR_COLOR, value );
			}
		}
		public static Color APPEARENCE_THEME_MINOR_COLOR
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_MINOR_COLOR ) )
				{
					return SetColor( Parameters.APPEARENCE_THEME_MINOR_COLOR, Color.FromArgb( 0xFF, 0x00, 0x71, 0xBC ) );
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_MINOR_COLOR ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_MINOR_COLOR, value );
			}
		}
		public static Color APPEARENCE_THEME_HORIZONTAL_RIBBON_COLOR
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_HORIZONTAL_RIBBON_COLOR ) )
				{
					return SetColor( Parameters.APPEARENCE_THEME_HORIZONTAL_RIBBON_COLOR, Color.FromArgb( 0xFF, 0x7A, 0xC9, 0x43 ) );
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_HORIZONTAL_RIBBON_COLOR ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_HORIZONTAL_RIBBON_COLOR, value );
			}
		}
		public static Color APPEARENCE_THEME_VERTICAL_RIBBON_COLOR
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_VERTICAL_RIBBON_COLOR ) )
				{
					return SetColor( Parameters.APPEARENCE_THEME_VERTICAL_RIBBON_COLOR, Color.FromArgb( 0xFF, 0xDC, 0x14, 0x3C ) );
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_VERTICAL_RIBBON_COLOR ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_VERTICAL_RIBBON_COLOR, value );
			}
		}

		#endregion

		#region Shades
		public static Color APPEARENCE_THEME_SHADES_10
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_SHADES_10 ) )
				{
					APPEARENCE_THEME_SHADES_10 = Colors.Black;
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_SHADES_10 ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_SHADES_10, value );
			}
		}
		public static Color APPEARENCE_THEME_SHADES_20
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_SHADES_20 ) )
				{
					APPEARENCE_THEME_SHADES_20 = Colors.Black;
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_SHADES_20 ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_SHADES_20, value );
			}
		}
		public static Color APPEARENCE_THEME_SHADES_30
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_SHADES_30 ) )
				{
					APPEARENCE_THEME_SHADES_30 = Colors.Black;
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_SHADES_30 ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_SHADES_30, value );
			}
		}
		public static Color APPEARENCE_THEME_SHADES_40
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_SHADES_40 ) )
				{
					APPEARENCE_THEME_SHADES_40 = Colors.Black;
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_SHADES_40 ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_SHADES_40, value );
			}
		}
		public static Color APPEARENCE_THEME_SHADES_50
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_SHADES_50 ) )
				{
					APPEARENCE_THEME_SHADES_50 = Colors.Black;
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_SHADES_50 ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_SHADES_50, value );
			}
		}
		public static Color APPEARENCE_THEME_SHADES_60
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_SHADES_60 ) )
				{
					APPEARENCE_THEME_SHADES_60 = Colors.Black;
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_SHADES_60 ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_SHADES_60, value );
			}
		}
		public static Color APPEARENCE_THEME_SHADES_70
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_SHADES_70 ) )
				{
					APPEARENCE_THEME_SHADES_70 = Colors.Black;
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_SHADES_70 ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_SHADES_70, value );
			}
		}
		public static Color APPEARENCE_THEME_SHADES_80
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_SHADES_80 ) )
				{
					APPEARENCE_THEME_SHADES_80 = Colors.Black;
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_SHADES_80 ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_SHADES_80, value );
			}
		}
		public static Color APPEARENCE_THEME_SHADES_90
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_SHADES_90 ) )
				{
					APPEARENCE_THEME_SHADES_90 = Colors.Black;
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_SHADES_90 ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_SHADES_90, value );
			}
		}

		public static Color APPEARENCE_THEME_RELATIVE_SHADES_COLOR
		{
			get
			{
				if ( !TestKey( Parameters.APPEARENCE_THEME_RELATIVE_SHADES_COLOR ) )
				{
					APPEARENCE_THEME_RELATIVE_SHADES_COLOR = Color.FromArgb( 0xFF, 0xDF, 0xDF, 0xDF );
				}
				return GetColorFromByte( GetValue<byte[]>( Parameters.APPEARENCE_THEME_RELATIVE_SHADES_COLOR ) );
			}
			set
			{
				SetColor( Parameters.APPEARENCE_THEME_RELATIVE_SHADES_COLOR, value );
			}
		}

		#endregion

		#endregion

		// Data
		public static bool DATA_IMAGE_SAVE_TO_MEDIA_LIBRARY
		{
			get
			{
				return GetValue<bool>( Parameters.DATA_IMAGE_SAVE_TO_MEDIA_LIBRARY );
			}
			set
			{
				SetParameter( Parameters.DATA_IMAGE_SAVE_TO_MEDIA_LIBRARY, value );
			}
		}

		public static bool DATA_CONNECTION_WIFI_ONLY
		{
			get
			{
				return GetValue<bool>( Parameters.DATA_CONNECTION_WIFI_ONLY );
			}
			set
			{
				SetParameter( Parameters.DATA_CONNECTION_WIFI_ONLY, value );
			}
		}

		// Protocol
		public static bool ENABLE_SERVER_SEL
		{
			get
			{
				return GetValue<bool>( Parameters.ENABLE_SERVER_SEL );
			}
			set
			{
				SetParameter( Parameters.ENABLE_SERVER_SEL, value );
			}
		}

		public static int SERVER_MIN_RANK
		{
			get
			{
				return GetValue<int>( Parameters.SERVER_MIN_RANK );
			}
			set
			{
				SetParameter( Parameters.SERVER_MIN_RANK, value );
			}
		}

		public static int SERVER_MAX_PING
		{
			get
			{
				return GetValue<int>( Parameters.SERVER_MAX_PING );
			}
			set
			{
				SetParameter( Parameters.SERVER_MAX_PING, value );
			}
		}

		public static string SERVER_OSD_URI
		{
			get
			{
				return GetValue<string>( Parameters.SERVER_OSD_URI );
			}
			set
			{
				SetParameter( Parameters.SERVER_OSD_URI, value );
			}
		}

		// Account
		public static string AUTH_TOKEN
		{
			get
			{
				return GetValue<string>( Parameters.AUTH_TOKEN );
			}
			set
			{
				SetParameter( Parameters.AUTH_TOKEN, value );
			}
		}

		public static bool ENABLE_ONEDRIVE
		{
			get
			{
				return GetValue<bool>( Parameters.ENABLE_ONEDRIVE );
			}
			set
			{
				SetParameter( Parameters.ENABLE_ONEDRIVE, value );
			}
		}

		public static string REVIEWS_SIGN
		{
			get
			{
				return GetValue<string>( Parameters.REVIEWS_SIGN );
			}
			set
			{
				SetParameter( Parameters.REVIEWS_SIGN, value );
			}
		}

		public static string LANGUAGE
		{
			get
			{
				return GetValue<string>( Parameters.LANGUAGE );
			}
			set
			{
				SetParameter( Parameters.LANGUAGE, value );
			}
		}

		public static bool LANGUAGE_TRADITIONAL
		{
			get
			{
				return GetValue<bool>( Parameters.LANGUAGE_TRADITIONAL );
			}
			set
			{
				SetParameter( Parameters.LANGUAGE_TRADITIONAL, value );
			}
		}

		public static bool MISC_TEXT_PATCH_SYNTAX
		{
			get
			{
				return GetValue<bool>( Parameters.MISC_TEXT_PATCH_SYNTAX );
			}
			set
			{
				SetParameter( Parameters.MISC_TEXT_PATCH_SYNTAX, value );
			}
		}

		public static bool MISC_CHUNK_SINGLE_VOL
		{
			get
			{
				return GetValue<bool>( Parameters.MISC_CHUNK_SINGLE_VOL );
			}
			set
			{
				SetParameter( Parameters.MISC_CHUNK_SINGLE_VOL, value );
			}
		}

		public static string MISC_COGNITIVE_API_KEY
		{
			get
			{
				return GetValue<string>( Parameters.MISC_COGNITIVE_API_KEY );
			}
			set
			{
				SetParameter( Parameters.MISC_COGNITIVE_API_KEY, value );
			}
		}

		private static Color GetColorFromByte( byte[] b )
		{
			Color c = new Color();
			c.A = b[0];
			c.R = b[1];
			c.G = b[2];
			c.B = b[3];
			return c;
		}

		private static Color SetColor( string ID, Color value )
		{
			byte[] b = new byte[] { value.A, value.R, value.G, value.B };
			SetParameter( ID, b );
			return value;
		}

	}
}