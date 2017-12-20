namespace GR.Model.Topics
{
	using Resources;
	using Settings;

	class Special : Feed
	{
		public Special()
		{
			if ( Shared.Storage.FileExists( FileLinks.ROOT_WTEXT + FileLinks.SPECIAL_LISTS ) )
				ParseXml( Shared.Storage.GetString( FileLinks.ROOT_WTEXT + FileLinks.SPECIAL_LISTS ) );
		}

		public Special( string Xml ) : base( Xml ) { }

		override protected bool WriteCaptionIfNew( string Value )
		{
			if ( Shared.Storage.FileChanged( Value, FileLinks.ROOT_WTEXT + FileLinks.SLISTS_LATEST ) )
			{
				Shared.Storage.WriteString( FileLinks.ROOT_WTEXT + FileLinks.SLISTS_LATEST, Value );
				return true;
			}
			return false;
		}

	}
}
