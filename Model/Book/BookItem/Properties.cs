using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;

namespace GR.Model.Book
{
	using Database.Models;
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

	abstract partial class BookItem : Book, IActiveData
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyChanged( params string[] Names )
		{
			if ( Worker.BackgroundOnly ) return;

			Worker.UIInvoke( () =>
			{
				// Must check each time after property changed is called
				// PropertyChanged may be null after event call
				foreach ( string Name in Names )
				{
					PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( Name ) );
				}
			} );
		}

		public static readonly string ID = typeof( BookItem ).Name;

		private static StringResBg Res { get { return new StringResBg( "Book" ); } }

		// For bookPool Indexing
		public int i { get; protected set; }

		// Used by derived class
		protected BookItem()
		{
			Others = new HashSet<string>();
		}

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
				// We need a different object reference each time for
				// notifying changes
				return Shared.Storage.FileExists( CoverPath )
					? new NameValue<string>( "Cover", CoverPath )
					: null;
			}
		}

		public string BannerPath
		{
			get { return FileLinks.ROOT_BANNER + Id + ".jpg"; }
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
				return Shared.Storage.FileChanged( Info.RecentUpdate, TOCDatePath );
			}
		}
	}
}