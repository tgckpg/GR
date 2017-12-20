﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.IO;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;
using Net.Astropenguin.Messaging;
using Net.Astropenguin.UI.Icons;

using libtaotu.Controls;
using libtaotu.Models.Interfaces;
using libtaotu.Models.Procedure;

namespace GR.Taotu
{
	using Model.Book.Spider;

	enum WListSub {
		Process = 1, Spider = 2
	}

	abstract class GrimoireListLoader : Procedure, ISubProcedure
	{
		public static readonly string ID = typeof( GrimoireListLoader ).Name;

		public WListSub? SubEdit { get; set; }

		public string ItemPattern { get; set; }
		public string ItemParam { get; set; }

		public bool HasSubProcs { get { return ItemProcs.HasProcedures; } }
		public bool HasBookSpider { get { return BookSpider.HasProcedures; } }

		public Uri BannerSrc { get; set; }
		public string BannerPath
		{
			get { return BannerSrc?.ToString(); }
			set
			{
				try
				{
					BannerSrc = new Uri( value );
					NotifyChanged( "BannerSrc" );
				}
				catch( Exception ) { }
			}
		}

		protected override Color BgColor { get { return Colors.Crimson; } }

		private ProcManager ItemProcs;
		private ProcManager BookSpider;

		public ProcManager SubProcedures
		{
			get { return SubEdit == WListSub.Process ? ItemProcs : BookSpider; }
			set { throw new InvalidOperationException(); }
		}

		public GrimoireListLoader()
			: base( ProcType.LIST )
		{
			ItemProcs = new ProcManager();
			BookSpider = new ProcManager();
		}

		public void SubEditComplete() { SubEdit = null; }

		public void SetProp( string PropName, string Val )
		{
			switch ( PropName )
			{
				case "ItemPattern": ItemPattern = Val; break;
				case "ItemParam": ItemParam = Val; break;
			}
		}

		public override void ReadParam( XParameter Param )
		{
			base.ReadParam( Param );

			ItemPattern = Param.GetValue( "ItemPattern" );
			ItemParam = Param.GetValue( "ItemParam" );
			BannerPath = Param.GetValue( "Banner" );

			ItemProcs = new ProcManager( Param.Parameter( "ItemProcs" ) );
			BookSpider = new ProcManager( Param.Parameter( "BookSpider" ) );
		}

		public override XParameter ToXParam()
		{
			XParameter Param = base.ToXParam();
			Param.SetValue( new XKey[]
			{
				new XKey( "ItemPattern", ItemPattern )
				, new XKey( "ItemParam", ItemParam )
				, new XKey( "Banner", BannerPath )
			} );

			XParameter EProc = ItemProcs.ToXParam();
			EProc.Id = "ItemProcs";
			Param.SetParameter( EProc );

			XParameter SProc = BookSpider.ToXParam();
			SProc.Id = "BookSpider";
			Param.SetParameter( SProc );

			return Param;
		}

		public void ImportSpider( XParameter SpiderDef )
		{
			ProcManager PM = new ProcManager( SpiderDef );
			BookSpider = PM;
			NotifyChanged( "HasBookSpider" );
		}

		public override async Task<ProcConvoy> Run( ProcConvoy Convoy )
		{
			Convoy = await base.Run( Convoy );

			ProcConvoy UsableConvoy;
			if ( !TryGetConvoy(
				out UsableConvoy
				, ( P, C ) =>
					C.Payload is IEnumerable<IStorageFile>
					|| C.Payload is IEnumerable<string>
					|| C.Payload is IStorageFile
					|| C.Payload is string
				) ) return Convoy;

			List<BookInstruction> SpItemList = null;

			// Search for the closest Instruction Set
			ProcConvoy SpiderInst = ProcManager.TracePackage(
				Convoy
				, ( P, C ) => C.Payload is IEnumerable<BookInstruction>
			);

			if ( SpiderInst != null )
			{
				SpItemList = ( List<BookInstruction> ) SpiderInst.Payload;
			}

			if ( SpItemList == null )
			{
				SpItemList = new List<BookInstruction>();
			}

			ProcPassThru PPass = new ProcPassThru( new ProcConvoy( this, SpItemList ) );
			ProcConvoy KnownBook = ProcManager.TracePackage( Convoy, ( P, C ) => C.Payload is BookInstruction );

			if ( UsableConvoy.Payload is IEnumerable<IStorageFile> )
			{
				IEnumerable<IStorageFile> ISFs = ( IEnumerable<IStorageFile> ) UsableConvoy.Payload;

				foreach ( IStorageFile ISF in ISFs )
				{
					string Content = await ISF.ReadString();
					await SearchBooks( SpItemList, PPass, KnownBook, Content );
				}
			}
			else if ( UsableConvoy.Payload is IEnumerable<string> )
			{
				IEnumerable<string> Contents = ( IEnumerable<string> ) UsableConvoy.Payload;

				foreach ( string Content in Contents )
				{
					await SearchBooks( SpItemList, PPass, KnownBook, Content );
				}
			}
			else if ( UsableConvoy.Payload is IStorageFile )
			{
				IStorageFile ISF = ( IStorageFile ) UsableConvoy.Payload;

				string Content = await ISF.ReadString();
				await SearchBooks( SpItemList, PPass, KnownBook, Content );
			}
			else // string
			{
				await SearchBooks( SpItemList, PPass, KnownBook, ( string ) UsableConvoy.Payload );
			}

			return new ProcConvoy( this, SpItemList );
		}

		private async Task SearchBooks( List<BookInstruction> ItemList, ProcPassThru PPass, ProcConvoy KnownBook, string Content )
		{
			ProcFind.RegItem RegParam = new ProcFind.RegItem( ItemPattern, ItemParam, true );

			if ( !RegParam.Validate() ) return;

			MatchCollection matches = RegParam.RegExObj.Matches( Content );
			StringResBg ste = new StringResBg( "Error" );

			foreach ( Match match in matches )
			{
				if ( HasSubProcs && RegParam.Valid )
				{
					string FParam = string.Format(
						RegParam.Format
						, match.Groups
							.Cast<Group>()
							.Select( g => g.Value )
							.ToArray()
					);

					ProcConvoy ItemConvoy = await ItemProcs.CreateSpider().Crawl( new ProcConvoy( PPass, FParam ) );

					string Id = await GetId( ItemConvoy );
					if ( string.IsNullOrEmpty( Id ) )
					{
						ProcManager.PanelMessage( this, ste.Str( "NoIdForBook" ), LogType.WARNING );
						continue;
					}

					ItemConvoy = ProcManager.TracePackage( ItemConvoy, ( P, C ) => C.Payload is BookInstruction );

					if ( !( ItemConvoy == null || ItemConvoy == KnownBook ) )
					{
						BookInstruction BInst = ( BookInstruction ) ItemConvoy.Payload;
						ItemList.Add( BInst );

						if ( HasBookSpider ) BInst.PlaceDefs( Id, BookSpider );
					}
					else
					{
						ProcManager.PanelMessage( this, ste.Str( "NotABook" ), LogType.WARNING );
					}
				}
			}
		}

		private async Task<string> GetId( ProcConvoy Convoy )
		{
			Convoy = ProcManager.TracePackage(
				Convoy, ( P, C ) =>
					C.Payload is IEnumerable<IStorageFile>
					|| C.Payload is IEnumerable<string>
					|| C.Payload is IStorageFile
					|| C.Payload is string
			);

			if ( Convoy == null ) return null;

			if ( Convoy.Payload is IEnumerable<IStorageFile> )
			{
				IEnumerable<IStorageFile> ISFs = ( IEnumerable<IStorageFile> ) Convoy.Payload;
				return await ISFs.FirstOrDefault()?.ReadString();
			}
			else if ( Convoy.Payload is IEnumerable<string> )
			{
				IEnumerable<string> Contents = ( IEnumerable<string> ) Convoy.Payload;
				return Contents.FirstOrDefault();
			}
			else if ( Convoy.Payload is IStorageFile )
			{
				IStorageFile ISF = ( IStorageFile ) Convoy.Payload;
				return await ISF.ReadString();
			}
			else // string
			{
				return ( string ) Convoy.Payload;
			}
		}

	}
}