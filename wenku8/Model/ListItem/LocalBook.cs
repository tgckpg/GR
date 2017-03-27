using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;
using Net.Astropenguin.Messaging;

using wenku8.Model.Book;

namespace wenku8.Model.ListItem
{
	using Config;
	using Resources;
	using Settings;
	using Storage;

	class LocalBook : ActiveItem
	{
		public static readonly string ID = typeof( LocalBook ).Name;

		public static bool IsTrad = Properties.LANGUAGE_TRADITIONAL;

		private static StringResources stx;

		public StorageFile File { get { return ( StorageFile ) RawPayload; } }

		public bool CanProcess { get; set; }
		public bool Processed { get; protected set; }
		public bool ProcessSuccess { get; protected set; }
		public bool Processing { get; protected set; }

		public bool CanReprocess { get { return CanProcess && !Processing; } }
		public bool ProcessFailed { get { return !( CanProcess || ProcessSuccess ); } }

		public bool CanFav
		{
			get
			{
				if ( ProcessSuccess ) return true;
				return IsFav;
			}
		}

		public string FavContextMenu
		{
			get
			{
				if ( stx == null ) stx = new StringResources( "AppBar" );
				return IsFav ? stx.Str( "FavOut" ) : stx.Str( "FavIn" );
			}
		}

		public string aid { get; protected set; }
		public bool IsFav { get; set; }

		public LocalBook( StorageFile File )
			: base( File.Name.ToCTrad(), File.Path, File )
		{
			Regex Reg = new Regex( "^(\\d+)" );

			Match m = Reg.Match( File.Name );
			if ( m.Groups[ 1 ].Success )
			{
				aid = m.Groups[ 1 ].Value;
				CanProcess = true;
			}
			else
			{
				Desc = "Invalid file name";
				CanProcess = false;
			}
		}

		public LocalBook() : base( null, null, null ) { }

		virtual protected async Task TestProcessed()
		{
			await Task.Run( () =>
			{
				LocalTextDocument Doc = new LocalTextDocument( aid );
				if ( Doc.IsValid )
				{
					Processed = File == null;
					CanProcess = !Processed;

					ProcessSuccess = true;

					Name = Doc.Title;
					Desc = Doc.Id;
				}
			} );
		}

		public static async Task<LocalBook> CreateAsync( string Id )
		{
			LocalBook Book = new LocalBook();
			Book.aid = Id;
			await Book.TestProcessed();
			return Book;
		}

		public async Task Process()
		{
			if ( !CanProcess || Processed || Processing ) return;
			Processing = true;
			NotifyChanged( "Processing", "CanReprocess" );

			MessageBus.OnDelivery += MessageBus_OnDelivery;
			try
			{
				ProcessSuccess = false;
				await Run();
				ProcessSuccess = true;
			}
			catch ( Exception ex )
			{
				Logger.Log( ID, ex.Message, LogType.ERROR );
				Name = ex.Message;
				Desc = "ERROR";
				CanProcess = false;
			}

			MessageBus.OnDelivery -= MessageBus_OnDelivery;
			Processed = true;
			Processing = false;
			NotifyChanged(
				"CanProcess", "ProcessSuccess"
				, "CanReprocess", "ProcessFailed"
				, "Processed", "Processing", "CanFav"
			);

			MessageBus.SendUI( GetType(), AppKeys.SP_PROCESS_COMP, this );
		}

		virtual protected async Task Run()
		{
			IInputStream s = await File.OpenSequentialReadAsync();

			string p;

			using ( StreamReader ms = new StreamReader( s.AsStreamForRead() ) )
			{
				p = ms.ReadToEnd();
			}

			LocalTextDocument L = await LocalTextDocument.ParseAsync( aid, p );

			Name = L.Title;
			Desc = "Saving ...";

			await L.Save();

			Desc = L.Id;
		}

		virtual protected void MessageBus_OnDelivery( Message MesgArgs )
		{
			if( MesgArgs.TargetType == typeof( LocalBook )
				&& MesgArgs.Payload.ToString() == aid )
			{
				Desc = MesgArgs.Content;
			}
		}

		virtual public void ToggleFav()
		{
			BookStorage BS = new BookStorage();
			if( BS.BookExist( aid ) )
			{
				BS.RemoveBook( aid );
				IsFav = false;
			}
			else
			{
				BS.SaveBook( aid, Name, "", "" );
				IsFav = true;
			}

			BS.SaveBookStorage();
			NotifyChanged( "IsFav", "FavContextMenu", "CanFav" );
		}

		virtual public void RemoveSource()
		{
			Shared.Storage.RemoveDir( FileLinks.ROOT_LOCAL_VOL + aid );
			Processed = false;
			ProcessSuccess = false;
			CanProcess = File != null;

			if( !CanProcess )
			{
				Desc = "Source is unavailable";
			}

			NotifyChanged( "ProcessSuccess", "Processed", "CanProcess", "CanFav" );
		}

		virtual public Task Reload() { return Task.Delay( 0 ); }
	}
}