namespace wenku8.Settings
{
	static class AppKeys
	{
        public const string
            // AvdDM
            DREG = FileLinks.ROOT_SETTING + FileLinks.ADM_DOWNLOAD_REG
            , DM_REQUESTID = "RequestID"
            , DM_PENDING = "Pending"
            , DM_ID = "ID"
            , DM_DESC = "Desc"
            , DM_REQUEST = "RequestUri"
            , DM_METHOD = "Method"
            , DM_FAILED_COUNT = "Failed"
            , DM_DEACTIVATED = "inactive"
            // Local Book Storage
            , LBS_ANCHOR = "anchor"
            , LBS_COLOR = "color"
            , LBS_DATE = "date"
            , LBS_DEL = "deleted"
            , LBS_CH = "chapter"
            , LBS_NEW = "new"
            , LBS_INDEX = "index"
            , LBS_AUM = "autoUpdate"
            , LBS_WSYNC = "wSync"
            , LBS_TIME = "t"

			// XML Root Nodes
			, LBS_BXML = "<LocalBookStorage />"
			, LBS_AXML = "<Anchors />"
			, AdvDM_FXML = "<AdvDM />"
			, TS_CXML = "<Themes />"

			// Globals
			, GLOBAL_NAME = "name"
			, GLOBAL_ID = "id"
			// Chapter id
			, GLOBAL_CID = "cid"
			// Volume id
			, GLOBAL_VID = "vid"
			// Reply id
			, GLOBAL_RID = "rid"
			// User id
			, GLOBAL_UID = "uid"

			, GLOBAL_AID = "aid"

			// Meta Tag
			, GLOBAL_META = "meta"

			// XML KEYS
			, XML_BINF_TITLE = "Title"
			, XML_BINF_AUTHOR = "Author"
			, XML_BINF_INTROPRV = "IntroPreview"
			, XML_BINF_BSTATUS = "BookStatus"
			, XML_BINF_LUPDATE = "LastUpdate"

			, XML_BMTA_PUSHCNT = "PushCount"
			, XML_BMTA_THITCNT = "TotalHitsCount"
			, XML_BMTA_DHITCNT = "DayHitsCount"
			, XML_BMTA_FAVCNT = "FavCount"
			, XML_BMTA_PRESSID = "PressId"
			, XML_BMTA_BLENGTH = "BookLength"
			, XML_BMTA_LSECTION = "LatestSection"

			, XML_UINFO_UNAME = "uname"
			, XML_UINFO_NNAWE = "nickname"
			, XML_UINFO_SCORE = "score"
			, XML_UINFO_EXP = "experience"
			, XML_UINFO_RANK = "rank"

            // Spider Props
			, BINF_INTRO = "Intro"
			, BINF_DATE = "Date"
			, BINF_OTHERS = "Others"
			, BINF_LENGTH = "Length"
            , BINF_STATUS = "Status"
			, BINF_PRESS = "Press"
			, BINF_ORGURL = "OriginalUrl"
            , BINF_COVER = "Cover"

			// AppGate Tags
			, APP_VERSION = "appver"
			, APP_REQUEST = "request"
			, APP_REQUEST_TOKEN = "timetoken"

            // Messaging
            , SH_SCRIPT_DATA = "SH_Script_Data"
            , SH_SHOW_GRANTS = "SH_Show_Grants"
            , HS_DECRYPT_FAIL = "HS_Decrypt_Fail"
            , HSC_DECRYPT_FAIL = "HSC_Decrypt_Fail"
            , SP_PROCESS_COMP = "Spider_Process_Comp"
			;
	}
}
