using Windows.UI.Xaml;

namespace wenku8.Model.Book
{
	class Volume
	{
		public string vid { get; set; }

		public Chapter[] ChapterList { get; set; }
		public string VolumeTitle { get; set; }
		public Visibility EndVisibility { get; set; }

		public bool VolumeTag
		{
			set { EndVisibility = value ? Visibility.Visible : Visibility.Collapsed; }
		}

        protected Volume() { }

		public Volume( string _vid, bool VTag, string VTitle, Chapter[] CList )
		{
			vid = _vid;
			VolumeTitle = VTitle;
			EndVisibility = VTag ? Visibility.Visible : Visibility.Collapsed;
			ChapterList = CList;
		}
	}
}