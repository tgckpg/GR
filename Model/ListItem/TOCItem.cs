namespace GR.Model.ListItem
{
	using Database.Models;
	sealed class TOCItem : TreeItem
	{
		public Volume Vol { get; private set; }
		public Chapter Ch { get; private set; }

		public TOCItem( Volume V )
			: base( V.Title, 0 )
		{
			Vol = V;
		}

		public TOCItem( Chapter C )
			: base( C.Title, 1 )
		{
			Ch = C;
		}
	}
}