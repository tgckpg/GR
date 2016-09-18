using System;

namespace wenku8.Settings
{
	static class FileLinks
	{
		public const string
		// Internal File Links
		SPECIAL_LISTS = "SpecialTopics.xml"
		, NEWS_LISTS = "news.xml"
		, PRESS_LISTF = "presslist.xml"
		, SLISTS_LATEST = "SpecialTopics.lat"
		, LOCAL_BOOK_STORAGE = "LocalBookStorage.xml"
		, THEME_SET = "ThemeSet.xml"
		, LAYOUT_BOOKINFOVIEW = "Layout_BookInfoView.xml"
		, LAYOUT_MAINPAGE = "Layout_MainPage.xml"
		, LAYOUT_NAVPAGE = "Layout_NavLPage.xml"
		, LAYOUT_CONTREADER = "Layout_ContentReader.xml"
		, READING_ANCHORS = "ReadingAnchors.xml"
        , READING_HISTORY = "ReadingHistory.xml"
        , ADM_DOWNLOAD_REG = "AdvDM_DReg.xml"
		, ADM_RUNTIME_REG = "AdvDM_RReg.xml"
		, EBWIN_DICT_REG = "EBWinDicts.xml"
		, SH_AUTH_REG = "SHAuths.xml"
        , SH_SCRIPT_ID = "SHScriptId.xml"

		, ROOT_CACHE = "Cache/"
		, ROOT_INTRO = "intro/"
		, ROOT_SHARED = "shared/"
		, ROOT_TILE = "shared/ShellContent/"
		, ROOT_COVER = "shared/ShellContent/Covers/"
		, ROOT_BANNER = "shared/ShellContent/Banners/"
		, ROOT_BACKGROUNDSERVICE = "shared/transfers/"
		, ROOT_IMAGE = "shared/transfers/Images/"
		, ROOT_VOLUME = "shared/transfers/Volumes/"
        , ROOT_LOCAL_VOL = "shared/transfers/LVolumes/"
        , ROOT_SPIDER_VOL = "shared/transfers/SVolumes/"
        , ROOT_ZSPIDER = "ZoneSpiders/"
		, ROOT_SETTING = "Settings/"
		, ROOT_AUTHMGR = "Settings/AuthManager/"
        , ROOT_EBWIN = "EBWin/"
		, ROOT_WTEXT = "WText/"
		, ROOT_ANCHORS = "Anchor/"
		;

        public static string GetVolumeRoot( string id )
		{
			return ROOT_VOLUME + id + "/";
		}
	}
}
