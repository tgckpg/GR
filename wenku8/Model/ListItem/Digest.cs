namespace wenku8.Model.ListItem
{
	sealed class Digests : ActiveItem
	{
		public int TreeLevel { get { return 1; } }

		public Digests( string name, string path )
			: base( name, null, path ) { }
	}
}
