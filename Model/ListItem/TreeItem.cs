namespace GR.Model.ListItem
{
	class TreeItem : Net.Astropenguin.DataModel.ActiveData
	{
		public int TreeLevel { get; protected set; }

		public string ItemTitle { get; protected set; }

		public TreeItem( string Name, int Level )
		{
			ItemTitle = Name;
			TreeLevel = Level;
		}
	}
}
