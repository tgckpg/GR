using Net.Astropenguin.Loaders;

namespace wenku8.System.Messages
{
    using Config;

	class GeneralMessage : StringResources
	{
		public GeneralMessage() : base() { }

		public string PUNC_ALREADY { get { return Str( "Already" ); } }
		public string PUNC_CANCEL { get { return Str( "Cancel" );  } }

		public string GetVersionNotification( string NewVersion, string TimetoExpire )
		{
			return Str( "VersionUpdate" ) + ": " + NewVersion + "\n"
				+ Str( "VersionUpdateCurrent" ) + ": "
				+ AppSettings.Version + "\n"
				+ Str( "VersionUpdateWill" )
				+ ( ( 2678400 - int.Parse( TimetoExpire ) )/86400 ).ToString()
				+ Str( "VersionUpdateExpire" );
		}
	}
}
