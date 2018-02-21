using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

using Net.Astropenguin.IO;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;
using Net.Astropenguin.Messaging;

namespace GR.Model.ListItem
{
	using Book;
	using Config;
	using GSystem;
	using Interfaces;
	using Resources;
	using Settings;

	class LocalBook : ActiveItem, IBookProcess
	{
		public static readonly string ID = typeof( LocalBook ).Name;

		public static bool IsTrad = Properties.LANGUAGE_TRADITIONAL;

		public StorageFile File { get { return ( StorageFile ) RawPayload; } }

		public bool CanProcess { get; set; }
		public bool Processed { get; protected set; }
		public bool ProcessSuccess { get; protected set; }
		public bool Processing { get; protected set; }

		public bool ProcessFailed { get { return !( CanProcess || ProcessSuccess ); } }

		private string _Zone;
		public string Zone
		{
			get => _Zone ?? ZoneId;
			set { _Zone = value; NotifyChanged( "Zone" ); }
		}

		public string ZoneId { get; protected set; }
		public string ZItemId { get; protected set; }

		private static StringResBg _stx;
		protected static StringResBg stx => _stx ?? ( _stx = new StringResBg( "AppResources", "LoadingMessage" ) );

		public LocalBook( StorageFile File )
			: base( File.Name, "", File.Path, "" )
		{
			Regex Reg = new Regex( "^(\\d+)" );

			RawPayload = File;

			ZoneId = AppKeys.ZLOCAL;
			CanProcess = true;

			Desc = stx.Text( "Ready" );

			Match m = Reg.Match( File.Name );
			if ( m.Groups[ 1 ].Success )
			{
				ZItemId = m.Groups[ 1 ].Value;
			}
			else
			{
				ZItemId = Utils.Md5( File.Name );
			}
		}

		public LocalBook() : base( null, null, null ) { }

		public LocalBook( Database.Models.Book Entry )
		{
			Name = Entry.Title;

			ZItemId = Entry.ZItemId;
			ZoneId = Entry.ZoneId;

			ProcessSuccess = true;
			Processed = true;
			CanProcess = false;
		}

		virtual protected async Task TestProcessed()
		{
			await Task.Run( () =>
			{
				LocalTextDocument Doc = new LocalTextDocument( ZItemId );
				if ( Doc.IsValid )
				{
					Processed = File == null;
					CanProcess = !Processed;

					ProcessSuccess = true;

					Name = Doc.Title;
					Desc = Doc.ZItemId;
				}
			} );
		}

		public async Task Process()
		{
			if ( !CanProcess || Processed || Processing ) return;
			Processing = true;
			NotifyChanged( "Processing" );

			MessageBus.Subscribe( this, MessageBus_OnDelivery );
			try
			{
				ProcessSuccess = false;
				await Run();
				ProcessSuccess = true;
			}
			catch ( OperationCanceledException ex )
			{
				Desc = ex.Message;
				goto ProcessComplete;
			}
			catch ( Exception ex )
			{
				Name = "ERROR";
				Desc = ex.Message;
				CanProcess = false;
			}

			Processed = true;
			MessageBus.SendUI( GetType(), AppKeys.SP_PROCESS_COMP, this );

			ProcessComplete:
			Processing = false;

			MessageBus.Unsubscribe( this, MessageBus_OnDelivery );
			NotifyChanged(
				"CanProcess", "ProcessSuccess", "ProcessFailed"
				, "Processed", "Processing"
			);
		}

		public void SetSource( StorageFile Source )
		{
			RawPayload = Source;
			CanProcess = true;
			Processed = false;

			Desc = stx.Text( "Ready" );
			Desc2 = Source.Path;
		}

		virtual protected async Task Run()
		{
			Desc = stx.Str( "ProgressIndicator_Message" );

			byte[] b = await File.ReadAllBytes();

			if ( await Shared.TC.ConfirmTranslate( ZItemId, File.Name ) )
			{
				MessageBus.Send( typeof( LocalBook ), "Translating ...", ZItemId );
				await Task.Run( () => b = Shared.TC.Translate( b ) );
			}

			LocalTextDocument L = await LocalTextDocument.ParseAsync( ZItemId, Encoding.UTF8.GetString( b ) );

			Name = L.Title;
			Desc = "Saving ...";

			await L.Save();

			Desc = string.Format( "{0} Volumes, {1} Chapters", L.Volumes.Count(), L.Volumes.Sum( x => x.Chapters.Count() ) );
		}

		virtual protected void MessageBus_OnDelivery( Message MesgArgs )
		{
			if( MesgArgs.TargetType == typeof( LocalBook )
				&& MesgArgs.Payload.ToString() == ZItemId )
			{
				Desc = MesgArgs.Content;
			}
		}

		virtual public void RemoveSource()
		{
			Shared.BooksDb.Delete( Database.Models.BookType.L, ZoneId, ZItemId );

			Processed = false;
			ProcessSuccess = false;
			if ( File == null )
			{
				CanProcess = false;
			}
			else
			{
				CanProcess = true;
				Name = File.Name;
				Desc2 = File.Path;
			}

			NotifyChanged( "ProcessSuccess", "Processed", "CanProcess" );
		}

		virtual public Task Reload() { return Task.Delay( 0 ); }
	}
}