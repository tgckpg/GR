using System;

namespace wenku8.Config
{
	static class Parameters
	{
		//// Appearance
		// Global
		public const string FIRST_TIME_RUN = "First_Time_Run";
        public const string INSTALLATION_INST = "Installation_Instance";
		public const string VERSION = "Version";
        public const string SMODE = "SMode";
		public const string LOG_LEVEL = "Log_Level";
		public const string ENABLE_SYSTEM_LOG = "Enable_System_Log";
		public const string ENABLE_RSYSTEM_LOG = "Enable_RSystem_Log";
		public const string ENABLE_SERVER_SEL = "Enable_Server_Selector";
		public const string RSYSTEM_LOG_ADDRESS = "RSystem_Log_Address";
		// ContentReader
		public const string CONTENTREADER_AUTOBOOKMARK = "ContentReader_Autobookmark";
        public const string CONTENTREADER_USEINERTIA = "ContentReader_UseInertia";
		public const string APPEARANCE_CONTENTREADER_FONTSIZE = "Appearance_ContentReader_FontSize";
        public const string APPEARANCE_CONTENTREADER_LEFTCONTEXT = "Appearance_ContentReader_LeftContext";
        public const string APPEARANCE_CONTENTREADER_EMBED_ILLUS = "Appearance_ContentReader_EmbedIllus";
		public const string APPEARANCE_CONTENTREADER_LINEHEIGHT = "Appearance_ContentReader_LineHeight";
		public const string APPEARANCE_CONTENTREADER_PARAGRAPHSPACING = "Appearance_ContentReader_ParagraphSpacing";
		public const string APPEARANCE_CONTENTREADER_BLOCKHEIGHT = "Appearance_ContentReader_BlockHeight";
		public const string APPEARANCE_CONTENTREADER_FONTCOLOR = "Appearance_ContentReader_FontColor";
		public const string APPEARANCE_CONTENTREADER_CLOCK_ARCOLOR = "Appearance_ContentReader_Clock_Arc_Hand_Color";
		public const string APPEARANCE_CONTENTREADER_CLOCK_HHCOLOR = "Appearance_ContentReader_Clock_Hour_Hand_Color";
		public const string APPEARANCE_CONTENTREADER_CLOCK_MHCOLOR = "Appearance_ContentReader_Clock_Minute_Hand_Color";
		public const string APPEARANCE_CONTENTREADER_CLOCK_SCOLOR = "Appearance_ContentReader_Clock_Scales_Color";
		public const string APPEARANCE_CONTENTREADER_ES_DCOLOR = "Appearance_ContentReader_EpStepper_DateColor";
		public const string APPEARANCE_CONTENTREADER_ES_SCOLOR = "Appearance_ContentReader_EpStepper_StepperColor";
		public const string APPEARANCE_CONTENTREADER_ES_BG = "Appearance_ContentReader_EpStepper_Background";
		public const string APPEARANCE_CONTENTREADER_ASSISTBG = "Appearance_ContentReader_AssistBg";
		public const string APPEARANCE_CONTENTREADER_NAVBG = "Appearance_ContentReader_NavBg";
		public const string APPEARANCE_CONTENTREADER_FONTWEIGHT = "Appearance_ContentReader_FontWeight";
		public const string APPEARANCE_CONTENTREADER_BACKGROUND = "Appearance_ContentReader_Background";
		public const string APPEARANCE_CONTENTREADER_SCROLLBAR = "Appearance_ContentReader_ScrollBar";
		public const string APPEARANCE_CONTENTREADER_HIDEHEADER = "Appearance_ContentReader_HideHeader";
		public const string APPEARANCE_CONTENTREADER_HIDEAPPBAR = "Appearance_ContentReader_HideAppBar";
		public const string APPEARANCE_CONTENTREADER_ENABLEREADINGANCHOR = "Appearance_ContentReader_EnableReadingAnchor";
		public const string APPEARANCE_CONTENTREADER_ENABLEDOUBLETAP = "Appearance_ContentReader_EnableDoubleTap";
		public const string APPEARANCE_CONTENTREADER_ENABLETAPBRUSH = "Appearance_ContentReader_EnableTapBrush";
		public const string APPEARANCE_CONTENTREADER_TAPBRUSHCOLOR = "Appearance_ContentReader_TapBrushColor";
		public const string APPEARANCE_PAGE_ORIENTATION = "Appearance_Page_Orientation";

		// Theme Color
		public const string APPEARENCE_THEME_PRESET_INDEX = "Appearance_Theme_Preset_Index";
		public const string APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_BACKGROUND = "Appearance_Theme_Text_Color_Relative_To_Background";
		public const string APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_MAJOR = "Appearance_Theme_Text_Color_Relative_To_Major";
		public const string APPEARENCE_THEME_SUBTLE_TEXT_COLOR = "Appearance_Theme_Subtle_Text_Color";
		public const string APPEARENCE_THEME_MAJOR_BACKGROUND_COLOR = "Appearance_Theme_Major_Background_Color";
		public const string APPEARENCE_THEME_MINOR_BACKGROUND_COLOR = "Appearance_Theme_Minor_Background_Color";
		public const string APPEARENCE_THEME_MAJOR_COLOR = "Appearance_Theme_Major_Color";
		public const string APPEARENCE_THEME_MINOR_COLOR = "Appearance_Theme_Minor_Color";
		public const string APPEARENCE_THEME_HORIZONTAL_RIBBON_COLOR = "Appearance_Theme_Horizontal_Ribbon_Color";
		public const string APPEARENCE_THEME_VERTICAL_RIBBON_COLOR = "Appearance_Theme_Vertical_Ribbon_Color";
		public const string APPEARENCE_THEME_SHADES_10 = "Appearance_Theme_Shades_10";
		public const string APPEARENCE_THEME_SHADES_20 = "Appearance_Theme_Shades_20";
		public const string APPEARENCE_THEME_SHADES_30 = "Appearance_Theme_Shades_30";
		public const string APPEARENCE_THEME_SHADES_40 = "Appearance_Theme_Shades_40";
		public const string APPEARENCE_THEME_SHADES_50 = "Appearance_Theme_Shades_10";
		public const string APPEARENCE_THEME_SHADES_60 = "Appearance_Theme_Shades_60";
		public const string APPEARENCE_THEME_SHADES_70 = "Appearance_Theme_Shades_70";
		public const string APPEARENCE_THEME_SHADES_80 = "Appearance_Theme_Shades_80";
		public const string APPEARENCE_THEME_SHADES_90 = "Appearance_Theme_Shades_90";

        public const string APPEARENCE_THEME_RELATIVE_SHADES_COLOR = "Appearence_Theme_Relative_Shades_Color";

		// Data - Image location
		public const string DATA_IMAGE_SAVE_TO_MEDIA_LIBRARY = "Data_Image_Save_To_Media_Library";
		public const string DATA_CONNECTION_WIFI_ONLY = "Data_Connection_WiFi_Only";

		// Account
		public const string AUTH_TOKEN = "Account_AuthToken";
		public const string ENABLE_ONEDRIVE = "Enable_OneDrive";

		public const string REVIEWS_SIGN = "REVIEWS_SIGN";

		// Language
		public const string LANGUAGE = "Language";
		public const string LANGUAGE_TRADITIONAL = "Language_traditional";

        // Protocol
        public const string SERVER_MIN_RANK = "ServerMinimumRank";
        public const string SERVER_MAX_PING = "ServerMaximumPing";
        public const string SERVER_OSD_URI = "ServerOnlineScriptDirUri";

        // Misc
		public const string MISC_TEXT_PATCH_SYNTAX = "Misc_Text_Patch_Syntax";
        public const string MISC_CHUNK_SINGLE_VOL = "Misc_Chunk_Single_Volume";
		public const string MISC_COGNITIVE_API_KEY = "Misc_Cognitive_Api_Key";
	}
}
