namespace wenku8.Model.Book
{
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
	}
}