using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;
using Net.Astropenguin.Messaging;

namespace GR.Resources
{
	using CompositeElement;
	using Database.Contexts;
	using GR.Model.ListItem;
	using Model;
	using Model.Book;
	using Model.REST;
	using Storage;

	static class Shared
	{
		public static GeneralStorage Storage;

		// Books Cache used by loaders
		public static BookItem CurrentBook;

		// The default settings by locale
		public static LocaleDefaults LocaleDefaults = new LocaleDefaults();

		public static SharersRequest ShRequest;

		private static StringResources LoadMesgRes;

		public static TradChinese TC;

		private static BooksContext _Books;
		public static BooksContext BooksDb => _Books ?? ( _Books = new BooksContext() );

		private static ZCacheContext _ZCache;
		public static ZCacheContext ZCacheDb => _ZCache ?? ( _ZCache = new ZCacheContext() );

		public static List<TreeItem> ExpZones = new List<TreeItem>();

		public static void LoadMessage( string MESG_ID, params string[] args )
		{
			Worker.UIInvoke( () =>
			{
				if ( LoadMesgRes == null ) LoadMesgRes = new StringResources( "LoadingMessage" );
				string mesg = LoadMesgRes.Str( MESG_ID );

				mesg = string.IsNullOrEmpty( mesg ) ? MESG_ID : mesg;

				if ( 0 < args.Length )
				{
					mesg = string.Format( mesg, args );
				}

				MessageBus.SendUI( typeof( LoadingMask ), mesg );
			} );
		}
	}
}