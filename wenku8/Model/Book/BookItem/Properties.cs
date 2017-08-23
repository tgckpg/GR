using System;
using System.Collections.Generic;
using System.IO;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Loaders;

namespace wenku8.Model.Book
{
	using Ext;
	using ListItem;
	using Resources;
	using Settings;
	using Storage;

	enum BookInfo
	{
		Others = 1,
		Title = 2,
		DailyHitsCount = 4,
		TotalHitsCount = 8,
		FavCount = 16,
		PushCount = 32,
		Date = 64,
		Author = 128,
		Press = 256,
		Status = 512,
		Length = 1024,
		LatestSection = 2048,
		Cover = 4096,
		Intro = 8192,
	}

	abstract partial class BookItem : ActiveData
	{
		public static readonly string ID = typeof( BookItem ).Name;

		private static StringResBg Res { get { return new StringResBg( "Book" ); } }

		// For bookPool Indexing
		public int i { get; protected set; }

		public BookItem( string id )
			:this()
		{
			// Initialize parameters
			AuthorRaw = RecentUpdateRaw = "";
			int j;
			if ( int.TryParse( id, out j ) )
			{
				Id = id;
				i = j;

				try
				{
					OriginalUrl = X.Call<string>( XProto.WRequest, "GetBookPage", Id );
				}
				catch( Exception ) { }
			}
			else
			{
				throw new ArgumentException();
			}

			IsFav = new BookStorage().BookExist( id );
		}

		// Used by derived class
		protected BookItem()
		{
			Others = new HashSet<string>();
		}

		virtual public string Id { get; protected set; }

		public string CoverPath
		{
			get { return FileLinks.ROOT_COVER + Id + ".jpg"; }
		}

		public NameValue<string> CoverExistsPath
		{
			get
			{
				// Since the path are always the same but the underlying
				// file may change
				// We need a different object reference is each time for
				// notifying changes
				return Shared.Storage.FileExists( CoverPath )
					? new NameValue<string>( "Cover", CoverPath )
					: null;
			}
		}

		public string CoverSrcUrl = null;

		public string BannerPath
		{
			get { return FileLinks.ROOT_BANNER + Id + ".jpg"; }
		}

		public string IntroPath
		{
			get { return FileLinks.ROOT_INTRO + Id + ".txt"; }
		}

		virtual public string VolumeRoot
		{
			get { return FileLinks.GetVolumeRoot( Id ); }
		}

		public string TOCPath
		{
			get { return VolumeRoot + "toc.txt"; }
		}

		public string TOCDatePath
		{
			get { return VolumeRoot + "toc.dsp"; }
		}

		virtual public bool NeedUpdate
		{
			get
			{
				return Shared.Storage.FileChanged( RecentUpdateRaw, TOCDatePath );
			}
		}
	}
}