using System.Linq;
using Windows.UI;

using Net.Astropenguin.IO;

namespace GR.Model.ListItem
{
	using GR.Database.Models;
	using GSystem;
	using Settings;

	sealed class BookmarkListItem : TreeItem
	{
		XParameter BindParam;
		Volume BindVolume;

		public Color TColor { get; private set; }

		public int AnchorIndex { get; private set; }

		public BookmarkListItem( Volume V, XParameter P )
			:base( P.GetValue( AppKeys.LBS_ANCHOR ), 1 )
		{
			BindVolume = V;
			BindParam = P;

			AnchorIndex = int.Parse( P.GetValue( AppKeys.LBS_INDEX ) );
			TColor = ThemeManager.StringColor( P.GetValue( AppKeys.LBS_COLOR ) );
		}

		public BookmarkListItem( Volume V )
			:base( V.Title, 0 )
		{
			BindVolume = V;
			AnchorIndex = -1;
		}

		public Chapter GetChapter()
		{
			if ( BindParam == null ) return null;
			string cid = BindParam.GetValue( AppKeys.GLOBAL_CID );

			return BindVolume.Chapters.First( ( Chapter C ) => C.Meta[ AppKeys.GLOBAL_CID ] == cid );
		}

		public bool IsItem( Volume V )
		{
			if ( BindParam == null ) return true;
			return false;
		}

		public bool IsItem( XParameter P )
		{
			if ( BindParam == null ) return false;
			return P.Equals( BindParam );
		}
	}
}