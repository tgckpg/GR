namespace GR.Model.Book
{
	using Database.Models;
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
			return Book.Entry.Type.HasFlag( BookType.L ) && Book.Entry.Type.HasFlag( BookType.W );
		}

		public static bool IsEx( this BookItem Book )
		{
			return Book.XTest( XProto.BookItemEx );
		}
	}
}