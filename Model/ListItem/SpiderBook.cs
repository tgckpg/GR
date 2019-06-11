using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using Net.Astropenguin.IO;
using Net.Astropenguin.Linq;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;
using Net.Astropenguin.Messaging;

using GFlow.Controls;
using GFlow.Crawler;
using GFlow.Models.Procedure;
using GR.GFlow;

namespace GR.Model.ListItem
{
	using Book.Spider;
	using Data;
	using Database.Models;
	using Database.Schema;
	using Interfaces;
	using Resources;
	using Settings;

	sealed class SpiderBook : LocalBook, IMetaSpider
	{
		const string FLAG_SUCCESS = "SP_SUCCESS";
		const string FLAG_CHAKRA = "SP_CHAKRA";

		public override string Name
		{
			get
			{
				if ( !Processed && CanProcess && string.IsNullOrEmpty( base.Name ) )
					base.Name = StringResources.Load().Text( "BookSpider" );

				return base.Name;
			}

			set { base.Name = value; }
		}

		public override string Desc
		{
			get
			{
				if ( !Processed && CanProcess && string.IsNullOrEmpty( base.Desc ) )
					base.Desc = StringResources.Load().Text( "UnprocessedItem" );

				return base.Desc;
			}

			set { base.Desc = value; }
		}

		// Determine if instructions has Chakra process inside
		// Which needs WebView, which needs UI Thread
		public bool HasChakra { get; private set; }

		private ProcManager ProcMan;
		private BookInstruction BInst;
		public XRegistry PSettings { get; private set; }

		private SpiderBook() { }

		public void MarkUnprocessed() => Processed = false;

		private void InitProcMan()
		{
			if ( ProcMan != null )
				return;

			ZData Data = Shared.BooksDb.Books
				.Where( x => x.ZoneId == ZoneId && x.ZItemId == ZItemId && x.Script != null )
				.Select( x => x.Script.Data )
				.FirstOrDefault();

			if ( Data != null )
			{
				if ( ReadGFlow( new MemoryStream( Data.BytesValue ) ) )
				{
					return;
				}
				// Fall off to legacy mode
				else
				{
					PSettings = PSettings ?? new XRegistry( Data.StringValue );
				}
			}

			// Legacy mode
			XParameter Param = PSettings.Parameter( "Procedures" );
			if ( Param == null )
			{
				Processed = true;
				CanProcess = false;
				ProcessSuccess = false;
				return;
			}

			ProcMan = new ProcManager();
			ProcMan.ReadParam( Param );
		}

		private async Task InitProcMan( IStorageFile ISF )
		{
			if ( ReadGFlow( await ISF.OpenStreamForReadAsync() ) )
				return;

			PSettings = new XRegistry( await ISF.ReadString(), null, false );
			InitProcMan();
		}

		public static async Task<SpiderBook> ImportFile( IStorageFile ISF, bool Save )
		{
			SpiderBook Book = new SpiderBook();

			await Book.InitProcMan( ISF );
			Book.ZoneId = AppKeys.ZLOCAL;
			Book.ZItemId = Book.ProcMan.GUID;

			await Book.TestProcessed();

			if ( Save && ( Book.CanProcess || Book.ProcessSuccess ) )
			{
				SScript ImportScript = new SScript() { Type = AppKeys.SS_BS };
				ImportScript.Data.WriteStream( await ISF.OpenStreamForReadAsync() );
			}

			return Book;
		}

		private bool ReadGFlow( Stream s )
		{
			try
			{
				using ( s )
				{
					DataContractSerializer DCS = new DataContractSerializer( typeof( GFDrawBoard ) );
					GFDrawBoard DBoard = DCS.ReadObject( s ) as GFDrawBoard;
					GFPathTracer Tracer = new GFPathTracer( DBoard );
					ProcMan = Tracer.CreateProcManager( DBoard.StartProc );
				}
				return true;
			}
			catch ( Exception ex )
			{
				Logger.Log( ID, ex.Message, LogType.WARNING );
			}

			return false;
		}

		public static XRegistry GetSettings( Book Book )
		{
			Shared.BooksDb.LoadRef( Book, x => x.Script );
			ProcManager PM = null;
			using ( Stream s = new MemoryStream( Book.Script.Data.BytesValue ) )
			{
				PM = ProcManager.Load( s );
			}

			if ( PM == null )
				return null;

			XRegistry XReg = new XRegistry( "<BookSpider />", null, false );
			switch ( Book.Script.Type )
			{
				case AppKeys.SS_BS:
					XReg.SetParameter( PM.ToXParam() );
					return XReg;
				case AppKeys.SS_ZS:
					GrimoireListLoader LL = PM.ProcList.FirstOrDefault( x => x is GrimoireListLoader ) as GrimoireListLoader;
					if ( LL != null && LL.HasBookSpider )
					{
						XParameter Procs = LL.ToXParam().Parameter( "BookSpider" );
						Procs.Id = "Procedures";
						XReg.SetParameter( Procs );
						return XReg;
					}
					break;
				default:
					throw new InvalidDataException( $"Unknown Script Type: {Book.Script.Type}" );
			}

			return null;
		}

		public static Task<SpiderBook> CreateSAsync( string ZItemId ) => CreateSAsync( null, ZItemId, null );
		public static async Task<SpiderBook> CreateSAsync( string ZoneId, string ZItemId, XParameter SpiderDef )
		{
			SpiderBook Book = new SpiderBook
			{
				ZoneId = ZoneId,
				ZItemId = ZItemId
			};

			if ( SpiderDef != null )
			{
				Book.PSettings = new XRegistry( "<ProcSpider />", null, false );
				Book.PSettings.SetParameter( SpiderDef );
			}

			await Book.TestProcessed();
			return Book;
		}

		public override async Task Reload()
		{
			ProcMan = null;
			throw new NotImplementedException();
			PSettings = new XRegistry( "<ProcSpider />" );
			ProcParameter.DestroyParams( PSettings );

			BInst?.ClearCover();

			await TestProcessed();
		}

		protected override async Task Run()
		{
			InitProcMan();
			ProceduralSpider Spider = ProcMan.CreateSpider();
			Spider.Log = SLog;
			Spider.PLog = SLog;

			// Payload is set from ZSViewSource
			string Payload = PSettings.Parameter( "METADATA" )?.GetValue( "payload" );

			ProcConvoy Convoy;

			// Since BInst saved by ZoneSpider has not parsed by the BookSpider yet
			// The ProcessSucess will always be null in this case hence BInst will be null
			if ( BInst != null && BInst.Entry.Meta.TryGetValue( AppKeys.XML_BMTA_PPVALUES, out string PPValues ) )
			{
				Convoy = ProcParameter.RestoreParams( PPValues.FromBase64ZString(), Payload );
			}
			else
			{
				Convoy = ProcParameter.RestoreParams( PSettings, Payload );
			}

			if ( Convoy.Dispatcher is ProcParameter )
			{
				Convoy = new ProcConvoy( new ProcPassThru( Convoy, ProcType.FEED_RUN ), null );
			}

			BInst = new BookInstruction( ZoneId, ZItemId );

			ProcPassThru PThru = new ProcPassThru( Convoy );

			// This values is set from PlaceDef in BookInstruction by ZoneSpider
			if ( BInst.Entry.Meta.ContainsKey( AppKeys.GLOBAL_SSID ) )
			{
				// Wrap another level
				PThru = new ProcPassThru( new ProcConvoy( PThru, BInst.Entry.Meta[ AppKeys.GLOBAL_SSID ] ) );
			}

			Convoy = await Spider.Crawl( new ProcConvoy( PThru, BInst ) );

			if ( Spider.LastException != null )
			{
				throw Spider.LastException;
			}

			HasChakra = ProcManager.TracePackage( Convoy, ( D, C ) => D is ProcChakra ) != null;

			ProcParameter.StoreParams( Convoy, x => BInst.Entry.Meta[ AppKeys.XML_BMTA_PPVALUES ] = x.AsBase64ZString() );
			Convoy = ProcManager.TracePackage( Convoy, ( D, C ) => C.Payload is BookInstruction );

			if ( Convoy == null ) throw new Exception( "Unable to find Book Info" );

			BInst = ( BookInstruction ) Convoy.Payload;

			Name = BInst.Title;
			Desc = BInst.LastUpdateDate;

			BInst.Info.Flags.Add( FLAG_SUCCESS );
			BInst.Info.Flags.Toggle( FLAG_CHAKRA, HasChakra );

			BInst.ZoneId = ZoneId;
			BInst.ZItemId = ZItemId;
			BInst.SaveInfo();
		}

		protected override Task TestProcessed()
		{
			return Task.Run( () =>
			{
				try
				{
					InitProcMan();

					if( ProcMan.ProcList.Count == 0 )
					{
						throw new ArgumentNullException( "ProcList is empty" );
					}

					BInst = new BookInstruction( ZoneId, ZItemId );
					ProcessSuccess = BInst.Info.Flags.Contains( FLAG_SUCCESS );

					if ( ProcessSuccess )
					{
						Name = BInst.Title;
						HasChakra = BInst.Info.Flags.Contains( FLAG_CHAKRA );

						Processed = Shared.BooksDb.SafeRun( Db => Db.Entry( BInst.Entry ).IsKeySet && Db.Volumes.Any( x => x.Book == BInst.Entry ) );
						if ( Processed )
						{
							Desc = StringResources.Load().Text( "RecordExist" );
						}
					}
					else
					{
						Shared.BooksDb.RemoveUnsaved( BInst.Entry );
						BInst = null;
					}

					CanProcess = true;
				}
				catch ( Exception ex )
				{
					Name = "ERROR";
					Desc = ex.Message;
					CanProcess = false;
					ProcessSuccess = false;
				}
			} );
		}

		public ProcConvoy GetPPConvoy() => ProcParameter.RestoreParams( PSettings, null );

		public void AssignId( string Id )
		{
			if ( ProcMan.GUID == Id ) return;
			throw new NotImplementedException();

			Book Entry = Shared.BooksDb.GetBook( ZoneId, ZItemId, BookType.S );
			Shared.BooksDb.LoadRef( Entry, x => x.Script );
			Entry.Script.OnlineId = Guid.Parse( Id );

			Shared.BooksDb.SaveBook( Entry );
		}

		public async Task<SpiderBook> Clone()
		{
			// Get the latest settings from original location
			XRegistry OSettings = new XRegistry( "<ProcSpider />", PSettings.Location );

			// We only need the "Procedures" node
			// Which also truncates all runtime infomation store from previous session
			XParameter Param = OSettings.Parameter( "Procedures" );

			// Apply the new Guid to the script
			XRegistry NDef = new XRegistry( "<ProcSpider />", "", false );
			Param.SetValue( new XKey( "Guid", Guid.NewGuid() ) );
			NDef.SetParameter( Param );

			throw new NotImplementedException();
			SpiderBook Bk = await ImportFile( null, true );
			Bk.Name = Name + " - Copy";

			return Bk;
		}

		public BookInstruction GetBook() => BInst;

		override protected void MessageBus_OnDelivery( Message Mesg )
		{
			if ( !( Mesg.Payload is PanelLog ) ) return;
			Desc = Mesg.Content;
		}

		override public void RemoveSource()
		{
			Book Entry = Shared.BooksDb.GetBook( ZoneId, ZItemId, BookType.S );
			if( Entry.Script != null )
			{
				Entry.Script = null;
				Shared.BooksDb.SaveBook( Entry );
			}

			Processed = false;
			ProcessSuccess = false;
			CanProcess = false;

			Desc = "Script is unavailable";
		}

		private void SLog( string Mesg, LogType LT ) => Desc = Mesg;
		private void SLog( Procedure P, string Mesg, LogType LT ) => Desc = Mesg;
	}
}