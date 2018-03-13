using System;

namespace GR.Config
{
	static class Parameters
	{
		//// Appearance
		// Global
		public const string FIRST_TIME_RUN = "First_Time_Run";
		public const string RESTORE_MODE = "Restore_Mode";
		public const string INSTALLATION_INST = "Installation_Instance";
		public const string VERSION = "Version";
		public const string LOG_LEVEL = "Log_Level";
		public const string ENABLE_SYSTEM_LOG = "Enable_System_Log";
		public const string ENABLE_RSYSTEM_LOG = "Enable_RSystem_Log";
		public const string ENABLE_SERVER_SEL = "Enable_Server_Selector";
		public const string RSYSTEM_LOG_ADDRESS = "RSystem_Log_Address";

		// Data - Image location
		public const string DATA_IMAGE_SAVE_TO_MEDIA_LIBRARY = "Data_Image_Save_To_Media_Library";
		public const string DATA_CONNECTION_WIFI_ONLY = "Data_Connection_WiFi_Only";

		// Account
		public const string REVIEWS_SIGN = "REVIEWS_SIGN";

		// Language
		public const string LANGUAGE = "Language";
		public const string LANGUAGE_TRADITIONAL = "Language_traditional";

		// Protocol
		public const string SERVER_MIN_RANK = "ServerMinimumRank";
		public const string SERVER_MAX_PING = "ServerMaximumPing";
		public const string SERVER_OSD_URI = "ServerOnlineScriptDirUri";

		// Misc
		public const string MISC_COGNITIVE_API_KEY = "Misc_Cognitive_Api_Key";
	}
}