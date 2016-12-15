using System;
using Windows.UI.Xaml;

namespace wenku8.Model.Book
{
	sealed class NonCollectedBook : BookItem
	{
		public NonCollectedBook( string name )
		{
			Title = name;
			Id = "-1";
		}

        public override Volume[] GetVolumes()
        {
            return new Volume[ 0 ];
        }
    }
}