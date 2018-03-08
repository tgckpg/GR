using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;

using Net.Astropenguin.Linq;

namespace GR
{
	using Database.Models;
	using Model.ListItem;
	using Resources;
	using Settings;

	class History
	{
		public static readonly string ID = typeof( History ).Name;

		public static Task CreateThumbnail( UIElement element, string PathId )
		{
			return Image.CaptureScreen( FileLinks.ROOT_READER_THUMBS + PathId, element, 120, 90 );
		}

		public History() { }

		public HistoryItem[] GetListItems()
		{
			return Shared.BooksDb.SafeRun(
				Db => Db.Books
				.Include( x => x.Info )
				.Where( x => x.LastAccess != null )
				.OrderByDescending( x => x.LastAccess )
				.Take( 15 )
				.ToArray()
			).Select( x => new HistoryItem( x ) ).ToArray();
		}

		public void Clear()
		{
			Shared.BooksDb.SafeRun( Db => Db.Books.Where( x => x.LastAccess != null ).ExecEach( x => { x.LastAccess = null; } ) );
			Shared.BooksDb.SaveChanges();
		}

		public class HistoryItem : ActiveItem
		{
			public HistoryItem( Book Bk )
				: base( Bk.Title, Bk.LastAccess?.ToLocalTime().ToString(), Bk )
			{
				Payload = FileLinks.ROOT_READER_THUMBS + Bk.ZoneId + "/" + Bk.ZItemId;
			}
		}

	}
}