using System;
using Windows.UI;

namespace GR.Config
{
	sealed class Properties : AppSettings
	{
		public static bool FIRST_TIME_RUN
		{
			get => GetValue<bool>( Parameters.FIRST_TIME_RUN );
			set => SetParameter( Parameters.FIRST_TIME_RUN, value );
		}

		public static bool RESTORE_MODE
		{
			get => GetValue<bool>( Parameters.RESTORE_MODE );
			set => SetParameter( Parameters.RESTORE_MODE, value );
		}

		public static string INSTALLATION_INST
		{
			get => GetValue<string>( Parameters.INSTALLATION_INST );
			set => SetParameter( Parameters.INSTALLATION_INST, value );
		}

		public static string VERSION
		{
			get => GetValue<string>( Parameters.VERSION );
			set => SetParameter( Parameters.VERSION, value );
		}

		public static bool ENABLE_SYSTEM_LOG
		{
			get => GetValue<bool>( Parameters.ENABLE_SYSTEM_LOG );
			set => SetParameter( Parameters.ENABLE_SYSTEM_LOG, value );
		}

		public static bool ENABLE_RSYSTEM_LOG
		{
			get => GetValue<bool>( Parameters.ENABLE_RSYSTEM_LOG );
			set => SetParameter( Parameters.ENABLE_RSYSTEM_LOG, value );
		}

		public static string LOG_LEVEL
		{
			get => GetValue<string>( Parameters.LOG_LEVEL );
			set => SetParameter( Parameters.LOG_LEVEL, value );
		}

		public static string RSYSTEM_LOG_ADDRESS
		{
			get => GetValue<string>( Parameters.RSYSTEM_LOG_ADDRESS );
			set => SetParameter( Parameters.RSYSTEM_LOG_ADDRESS, value );
		}

		// Data
		public static bool DATA_IMAGE_SAVE_TO_MEDIA_LIBRARY
		{
			get => GetValue<bool>( Parameters.DATA_IMAGE_SAVE_TO_MEDIA_LIBRARY );
			set => SetParameter( Parameters.DATA_IMAGE_SAVE_TO_MEDIA_LIBRARY, value );
		}

		public static bool DATA_CONNECTION_WIFI_ONLY
		{
			get => GetValue<bool>( Parameters.DATA_CONNECTION_WIFI_ONLY );
			set => SetParameter( Parameters.DATA_CONNECTION_WIFI_ONLY, value );
		}

		// Protocol
		public static bool ENABLE_SERVER_SEL
		{
			get => GetValue<bool>( Parameters.ENABLE_SERVER_SEL );
			set => SetParameter( Parameters.ENABLE_SERVER_SEL, value );
		}

		public static int SERVER_MIN_RANK
		{
			get => GetValue<int>( Parameters.SERVER_MIN_RANK );
			set => SetParameter( Parameters.SERVER_MIN_RANK, value );
		}

		public static int SERVER_MAX_PING
		{
			get => GetValue<int>( Parameters.SERVER_MAX_PING );
			set => SetParameter( Parameters.SERVER_MAX_PING, value );
		}

		public static string REVIEWS_SIGN
		{
			get => GetValue<string>( Parameters.REVIEWS_SIGN );
			set => SetParameter( Parameters.REVIEWS_SIGN, value );
		}

		public static string LANGUAGE
		{
			get => GetValue<string>( Parameters.LANGUAGE );
			set => SetParameter( Parameters.LANGUAGE, value );
		}

		public static bool LANGUAGE_TRADITIONAL
		{
			get => GetValue<bool>( Parameters.LANGUAGE_TRADITIONAL );
			set => SetParameter( Parameters.LANGUAGE_TRADITIONAL, value );
		}

		public static string MISC_COGNITIVE_API_KEY
		{
			get => GetValue<string>( Parameters.MISC_COGNITIVE_API_KEY );
			set => SetParameter( Parameters.MISC_COGNITIVE_API_KEY, value );
		}

	}
}