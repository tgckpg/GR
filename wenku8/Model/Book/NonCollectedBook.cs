using Windows.UI.Xaml;

namespace wenku8.Model.Book
{
	class NonCollectedBook : BookItem
	{
		public NonCollectedBook( string name )
		{
			Title = name;
			Id = "-1";
		}
	}
}