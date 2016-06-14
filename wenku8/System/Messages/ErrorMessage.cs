using Net.Astropenguin.Loaders;

namespace wenku8.System.Messages
{
	class ErrorMessage : StringResources
	{
		public ErrorMessage() : base( "Error" ) { }

		public string UPDATE { get { return Str( "Update" ); } }
		public string DOWNLOAD { get { return Str( "Download" ); } }
		public string PARSE { get { return Str( "ParseFailed" ); } }
		public string VERIFICATION {  get { return Str( "Verification" ); } }
	}

}
