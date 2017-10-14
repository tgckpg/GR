﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Messaging;

namespace wenku8.Resources
{
	using CompositeElement;
	using Model;
	using Model.Book;
	using Model.REST;
	using Storage;

	static class Shared
	{
		public static GeneralStorage Storage;

		// Books Cache used by loaders
		public static BookPool BooksCache = new BookPool( 5 );
		public static BookItem CurrentBook;

		// The default settings by locale
		public static LocaleDefaults LocaleDefaults = new LocaleDefaults();

		public static SharersRequest ShRequest;

		private static StringResources LoadMesgRes;

		public static TradChinese TC;

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