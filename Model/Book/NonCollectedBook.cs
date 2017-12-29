using System;
using Windows.UI.Xaml;

namespace GR.Model.Book
{
	sealed class NonCollectedBook : BookItem
	{
		public NonCollectedBook( string name )
			: base( null, Database.Models.BookType.W, null )
		{
			Title = name;
		}
	}
}