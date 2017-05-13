namespace wenku8.Model.Book
{
	using Ext;
	using Spider;

	static class BookItemExt
	{
		public static bool IsLocal( this BookItem Book )
		{
			return Book is LocalTextDocument;
		}

		public static bool IsSpider( this BookItem Book )
		{
			return Book is BookInstruction;
		}

		public static bool IsDeathblow( this BookItem Book )
		{
			return Book.XTest( XProto.DeathBook );
		}

		public static bool IsEx( this BookItem Book )
		{
			return Book.XTest( XProto.BookItemEx );
		}
	}
}