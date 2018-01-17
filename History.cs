using GR.Database.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;

using Net.Astropenguin.Linq;

namespace GR
{
	using Model.ListItem;
	using Resources;
	using Settings;

	class History
	{
		public static readonly string ID = typeof( History ).Name;

		public const string SettingsFile = FileLinks.ROOT_SETTING + FileLinks.READING_HISTORY;

		public static Task CreateThumbnail( UIElement element, int BookId )
		{
			return Image.CaptureScreen( FileLinks.ROOT_READER_THUMBS + BookId, element, 120, 90 );
		}

		public History()
		{
		}

		public ActiveItem[] GetListItems()
		{
			IQueryable<Book> Books = Shared.BooksDb.Books.Where( x => x.LastAccess != null ).OrderByDescending( x => x.LastAccess );
			return Books.Remap( x => new HistoryItem( x ) );
		}

		public void Clear()
		{
			Shared.BooksDb.Books.Where( x => x.LastAccess != null ).ExecEach( x => { x.LastAccess = null; } );
			Shared.BooksDb.SaveChanges();
		}

		public class HistoryItem : ActiveItem
		{
			public HistoryItem( Book Bk )
				: base( Bk.Title, Bk.LastAccess?.ToLocalTime().ToString(), Bk )
			{
				Payload = Bk.Id.ToString();
			}
		}

	}
}