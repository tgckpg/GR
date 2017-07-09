using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;

namespace wenku8
{
	using Resources;
	using Settings;
	using Model.ListItem;
	using Model.Book;

	class History
	{
		public static readonly string ID = typeof( History ).Name;

		public const string SettingsFile = FileLinks.ROOT_SETTING + FileLinks.READING_HISTORY;
		private XRegistry Registry;

		public static Task CreateThumbnail( UIElement element, string BookId )
		{
			return Image.CaptureScreen( FileLinks.ROOT_READER_THUMBS + BookId, element, 120, 90 );
		}

		public History()
		{
			Registry = new XRegistry( AppKeys.LBS_AXML, SettingsFile );
		}

		public void Push( BookItem b )
		{
			DateTime d = DateTime.Now.ToUniversalTime();
			Logger.Log( ID, "Date: " + d.ToString(), LogType.DEBUG );
			Registry.SetParameter(
				b.Id
				, new XKey[] {
					new XKey( AppKeys.GLOBAL_NAME, b.Title )
					, new XKey( AppKeys.LBS_DATE, d.ToString() )
				}
			);
			Registry.Save();
		}

		public ActiveItem[] GetListItems()
		{
			XParameter[] allHistory = Registry.Parameters();
			Array.Sort( allHistory, delegate ( XParameter a, XParameter b ) {
				DateTime date = DateTime.Parse( a.GetValue( AppKeys.LBS_DATE ) );
				DateTime dateb = DateTime.Parse( b.GetValue( AppKeys.LBS_DATE ) );
				return dateb.CompareTo( date );
			} );
			ActiveItem[] ll = new ActiveItem[ allHistory.Length ];

			int l = allHistory.Length;
			for( int i = 0; i < l; i ++ )
			{
				DateTime date = DateTime.Parse( allHistory[ i ].GetValue( AppKeys.LBS_DATE ) );
				ll[i] = new ActiveItem(
					allHistory[i].GetValue( AppKeys.GLOBAL_NAME )
					, date.ToLocalTime().ToString()
					, allHistory[i].Id
				);
			}

			return ll;
		}

		public void Clear()
		{
			Shared.Storage.DeleteFile( SettingsFile );
		}
	}
}