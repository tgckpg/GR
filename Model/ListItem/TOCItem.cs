namespace GR.Model.ListItem
{
	using Database.Models;
	sealed class TOCItem : TreeItem
	{
		private Volume Vol;
		private Chapter Ch;

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

		public bool IsItem( Chapter C )
		{
			if ( Ch == null ) return false;
			return Ch.Equals( C );
		}

		public bool IsItem( Volume V )
		{
			if ( Vol == null ) return false;
			return Vol.Equals( V );
		}

		public Chapter GetChapter()
		{
			if ( Ch == null )
			{
				if ( Vol.Chapters.Count == 0 ) return null;
				return Vol.Chapters[ 0 ];
			}
			return Ch;
		}
	}
}