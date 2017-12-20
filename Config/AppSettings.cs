using System;
using System.Linq;
using System.ComponentModel;
using Windows.ApplicationModel;
using Windows.Networking.Connectivity;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;
using Windows.UI;

using Net.Astropenguin.Logging;

namespace GR.Config
{
	using AdvDM;
	using Resources;

	class AppSettings
	{
		public static readonly string ID = typeof( AppSettings ).Name;

		private static ApplicationDataContainer ISettings = ApplicationData.Current.LocalSettings;
		public static event PropertyChangedEventHandler PropertyChanged;

		public static string Version
		{
			get
			{
				return string.Format(
					"{0}.{1}.{2}.{3}"
					, Package.Current.Id.Version.Major
					, Package.Current.Id.Version.Minor
					, Package.Current.Id.Version.Build
					, Package.Current.Id.Version.Revision
				);
			}
		}

		public static string DeviceName { get; private set; }
		public static string DeviceId { get { return Properties.INSTALLATION_INST; } }

		public static string FamilyName
		{
			get { return Package.Current.Id.FamilyName; }
		}

		public static string SimpVersion
		{
			get
			{
				return string.Format(
					"{0}.{1}.{2}"
					, Package.Current.Id.Version.Major
					, Package.Current.Id.Version.Minor
					, Package.Current.Id.Version.Build
				);
			}
		}

		public static void SetParameter<T>( string Key, T value )
		{
			if ( ISettings.Values.ContainsKey( Key ) )
				ISettings.Values[ Key ] = value;
			else
				ISettings.Values.Add( Key, value );

			PropertyChanged?.Invoke( null, new PropertyChangedEventArgs( Key ) );
		}

		protected static T GetValue<T>( string Key )
		{
			if ( TestKey( Key ) )
				return ( T ) ISettings.Values[ Key ];
			return default( T );
		}

		protected static bool TestKey( string Key )
		{
			return ISettings.Values.ContainsKey( Key );
		}

		public static void Initialize()
		{
			SetDeviceInfo();

			//// Global
			if ( !TestKey( Parameters.ENABLE_SYSTEM_LOG ) )
				Properties.ENABLE_SYSTEM_LOG = false;

			if ( !TestKey( Parameters.ENABLE_RSYSTEM_LOG ) )
				Properties.ENABLE_RSYSTEM_LOG = false;

			if ( !TestKey( Parameters.SMODE ) )
				Properties.SMODE = 0;

#if RELEASE || Beta
			// Force disable logging
			Properties.ENABLE_SYSTEM_LOG = false;
			Properties.ENABLE_RSYSTEM_LOG = false;
#endif

			if ( !TestKey( Parameters.LOG_LEVEL ) )
				Properties.LOG_LEVEL = "INFO";
			if ( !TestKey( Parameters.RSYSTEM_LOG_ADDRESS ) )
				Properties.RSYSTEM_LOG_ADDRESS = "127.0.0.1";

			//// Local Book shelf

			//// Appearance section

			// Content Reader
			if ( !TestKey( Parameters.CONTENTREADER_AUTOBOOKMARK ) )
				Properties.CONTENTREADER_AUTOBOOKMARK = true;
			// Rememeber to adjust the optimal BlockHeight when changing this value
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_FONTSIZE ) )
				Properties.APPEARANCE_CONTENTREADER_FONTSIZE = 20;
			// This value should be tested across different device, otherwise leave it to 0
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_BLOCKHEIGHT ) )
				Properties.APPEARANCE_CONTENTREADER_BLOCKHEIGHT = 21.7;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_FONTWEIGHT ) )
				Properties.APPEARANCE_CONTENTREADER_FONTWEIGHT = Windows.UI.Text.FontWeights.Normal;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_EMBED_ILLUS ) )
				Properties.APPEARANCE_CONTENTREADER_EMBED_ILLUS = false;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_LEFTCONTEXT ) )
				Properties.APPEARANCE_CONTENTREADER_LEFTCONTEXT = true;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_LINEHEIGHT ) )
				Properties.APPEARANCE_CONTENTREADER_LINEHEIGHT = 5;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_NAVBG ) )
				Properties.APPEARANCE_CONTENTREADER_NAVBG = Properties.APPEARENCE_THEME_MAJOR_COLOR;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_ASSISTBG ) )
				Properties.APPEARANCE_CONTENTREADER_ASSISTBG = Colors.Black;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_PARAGRAPHSPACING ) )
				Properties.APPEARANCE_CONTENTREADER_PARAGRAPHSPACING = 32;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_BACKGROUND ) )
				Properties.APPEARANCE_CONTENTREADER_BACKGROUND = Colors.White;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_SCROLLBAR ) )
				Properties.APPEARANCE_CONTENTREADER_SCROLLBAR = Color.FromArgb( 0x3C, 0xFF, 0xFF, 0xFF );
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_FONTCOLOR ) )
				Properties.APPEARANCE_CONTENTREADER_FONTCOLOR = Color.FromArgb( 0xE5, 0, 0, 0 );
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_HIDEAPPBAR ) )
				Properties.APPEARANCE_CONTENTREADER_HIDEAPPBAR = true;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_HIDEHEADER ) )
				Properties.APPEARANCE_CONTENTREADER_HIDEHEADER = true;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_ENABLEREADINGANCHOR ) )
				Properties.APPEARANCE_CONTENTREADER_ENABLEREADINGANCHOR = true;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_ENABLEDOUBLETAP ) )
				Properties.APPEARANCE_CONTENTREADER_ENABLEDOUBLETAP = false;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_ENABLETAPBRUSH ) )
				Properties.APPEARANCE_CONTENTREADER_ENABLETAPBRUSH = true;
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_TAPBRUSHCOLOR ) )
				Properties.APPEARANCE_CONTENTREADER_TAPBRUSHCOLOR = Color.FromArgb( 0xFF, 0x3F, 0xA9, 0xF5 );

			// Clock
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_CLOCK_ARCOLOR ) )
				Properties.APPEARANCE_CONTENTREADER_CLOCK_ARCOLOR = Color.FromArgb( 0x50, 0xFF, 0xFF, 0xFF );
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_CLOCK_HHCOLOR ) )
				Properties.APPEARANCE_CONTENTREADER_CLOCK_HHCOLOR = Color.FromArgb( 0x78, 0xEE, 0xEB, 0xAA );
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_CLOCK_MHCOLOR ) )
				Properties.APPEARANCE_CONTENTREADER_CLOCK_MHCOLOR = Color.FromArgb( 0x50, 0xFF, 0xFF, 0xFF );
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_CLOCK_SCOLOR ) )
				Properties.APPEARANCE_CONTENTREADER_CLOCK_SCOLOR = Color.FromArgb( 0x3C, 0xFF, 0xFF, 0xFF );

			// EpisodeStepper
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_ES_DCOLOR ) )
				Properties.APPEARANCE_CONTENTREADER_ES_DCOLOR = Color.FromArgb( 0xC7, 0xFF, 0xFF, 0xFF );
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_ES_SCOLOR ) )
				Properties.APPEARANCE_CONTENTREADER_ES_SCOLOR = Color.FromArgb( 0xFF, 0xFF, 0xFF, 0xFF );
			if ( !TestKey( Parameters.APPEARANCE_CONTENTREADER_ES_BG ) )
				Properties.APPEARANCE_CONTENTREADER_ES_BG = Color.FromArgb( 0x70, 0x00, 0x00, 0x00 );


			// Themes, default Light Theme
			if ( !TestKey( Parameters.APPEARENCE_THEME_PRESET_INDEX ) )
				Properties.APPEARENCE_THEME_PRESET_INDEX = 1;

			//// Data section

			// Connection
			if ( !TestKey( Parameters.DATA_CONNECTION_WIFI_ONLY ) )
				Properties.DATA_CONNECTION_WIFI_ONLY = false;

			// Image Viewer
			if ( !TestKey( Parameters.DATA_IMAGE_SAVE_TO_MEDIA_LIBRARY ) )
				Properties.DATA_IMAGE_SAVE_TO_MEDIA_LIBRARY = false;

			if ( !TestKey( Parameters.LANGUAGE ) )
			{
				string LangCode = Windows.System.UserProfile.GlobalizationPreferences.Languages[ 0 ];
				Logger.Log( ID, "The dectected lang code is: " + LangCode, LogType.INFO );

				switch ( LangCode )
				{
					/*
					case "zh-Hant-TW":
					case "zh-Hant-MO":
					case "zh-Hant-HK":
					case "zh-Hans-CN":
					case "zh-Hans-SG":
					*/
					case "ja":
						Properties.LANGUAGE = "ja";
						Properties.LANGUAGE_TRADITIONAL = false;
						break;
					default:
						if ( LangCode.Contains( "-Hans-" ) )
						{
							Properties.LANGUAGE = "zh-CN";
							Properties.LANGUAGE_TRADITIONAL = false;
						}
						else if ( LangCode.Contains( "-Hant-" ) )
						{
							Properties.LANGUAGE = "zh-TW";
							Properties.LANGUAGE_TRADITIONAL = true;
						}
						else
						{
							// Default
							Properties.LANGUAGE = "en-US";
							Properties.LANGUAGE_TRADITIONAL = false;
						}
						break;
				}

				// Safe net setting
				if ( !TestKey( Parameters.LANGUAGE_TRADITIONAL ) )
				{
					Properties.LANGUAGE_TRADITIONAL = true;
				}
			}

			switch ( Properties.LANGUAGE )
			{
				case "zh-CN":
				case "zh-TW":
				case "ja":
					Shared.LocaleDefaults[ "ContentReader.IsHorizontal" ] = true;
					Shared.LocaleDefaults[ "ContentReader.IsRightToLeft" ] = true;
					Shared.LocaleDefaults[ "ContentReader.UseInertia" ] = false;
					Shared.LocaleDefaults[ "BookInfoView.IsRightToLeft" ] = true;
					Shared.LocaleDefaults[ "BookInfoView.HorizontalTOC" ] = true;
					break;

				case "en-US":
				default:
					Shared.LocaleDefaults[ "BookInfoView.HorizontalTOC" ] = false;
					Shared.LocaleDefaults[ "ContentReader.IsHorizontal" ] = false;
					Shared.LocaleDefaults[ "ContentReader.IsRightToLeft" ] = false;
					Shared.LocaleDefaults[ "BookInfoView.IsRightToLeft" ] = false;
					Shared.LocaleDefaults[ "ContentReader.UseInertia" ] = true;
					break;
			}

			//// Account section
			if ( !TestKey( Parameters.ENABLE_ONEDRIVE ) )
				Properties.ENABLE_ONEDRIVE = false;

			if ( !TestKey( Parameters.ENABLE_SERVER_SEL ) )
				Properties.ENABLE_SERVER_SEL = false;

			if ( !TestKey( Parameters.SERVER_MIN_RANK ) )
				Properties.SERVER_MIN_RANK = 10;

			if ( !TestKey( Parameters.SERVER_MAX_PING ) )
				Properties.SERVER_MAX_PING = 800;

			if ( !TestKey( Parameters.SERVER_OSD_URI ) )
			{
#if DEBUG && !ARM
				Properties.SERVER_OSD_URI = "http://w10srv.botanical.astropenguin.net/";
#else
				Properties.SERVER_OSD_URI = "https://w10srv.astropenguin.net/";
#endif
			}

			// Misc
			if ( !TestKey( Parameters.MISC_TEXT_PATCH_SYNTAX ) )
				Properties.MISC_TEXT_PATCH_SYNTAX = true;

			if ( !TestKey( Parameters.MISC_CHUNK_SINGLE_VOL ) )
				Properties.MISC_CHUNK_SINGLE_VOL = true;

			if ( !TestKey( Parameters.FIRST_TIME_RUN ) )
				Properties.FIRST_TIME_RUN = true;

			if ( !TestKey( Parameters.INSTALLATION_INST ) )
				Properties.INSTALLATION_INST = Guid.NewGuid().ToString();

			if ( !TestKey( Parameters.CONTENTREADER_USEINERTIA ) )
				Properties.CONTENTREADER_USEINERTIA = Shared.LocaleDefaults.Get<bool>( "ContentReader.UseInertia" );

			// Cognitive API
			if ( !TestKey( Parameters.MISC_COGNITIVE_API_KEY ) )
				Properties.MISC_COGNITIVE_API_KEY = "";

			BingService.SetApiKey( Properties.MISC_COGNITIVE_API_KEY );

			GSystem.LogControl.SetFilter( Properties.LOG_LEVEL );
			Logger.Log( ID, "Initilizated", LogType.INFO );
		}

		private static void SetDeviceInfo()
		{
			DeviceName = NetworkInformation.GetHostNames().FirstOrDefault()?.DisplayName;
			if ( string.IsNullOrEmpty( DeviceName ) )
			{
				EasClientDeviceInformation Info = new EasClientDeviceInformation();
				DeviceName = Info.FriendlyName;
			}
		}
	}
}