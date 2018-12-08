using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;
using Net.Astropenguin.IO;
using Net.Astropenguin.Helpers;
using Net.Astropenguin.UI.Icons;

using GFlow.Controls;
using GFlow.Models.Interfaces;
using GFlow.Models.Procedure;

namespace GR.GFlow
{
	using Database.Schema;
	using CompositeElement;
	using Model.Book.Spider;
	using Resources;

	sealed class TongWenTang : Procedure
	{
		public TongWenTang() : base( ProcType.GENERIC ) { }

		public override async Task<ProcConvoy> Run( ICrawler Crawler, ProcConvoy Convoy )
		{
			if ( !Shared.Conv.DoTraditional ) return Convoy;

			// Book Instruction Translate
			if ( Convoy.Dispatcher is GrimoireExtractor && Convoy.Payload is BookInstruction )
			{
				BookInstruction Book = ( BookInstruction ) Convoy.Payload;

				Book.Entry.EachProperty<string>( _Translate );
				Book.Entry.Info.EachProperty<string>( _Translate );

				return Convoy;
			}

			Crawler.PLog( this, Res.RSTR( "IncomingCheck" ), LogType.INFO );

			ProcConvoy UsableConvoy = ProcManager.TracePackage(
				Convoy, ( P, C ) =>
					C.Payload is IEnumerable<IStorageFile>
					|| C.Payload is IEnumerable<string>
					|| C.Payload is IStorageFile
					|| C.Payload is string
			);

			if ( UsableConvoy == null ) return Convoy;

			if ( UsableConvoy.Payload is IEnumerable<IStorageFile> )
			{
				foreach ( IStorageFile ISF in ( ( IEnumerable<IStorageFile> ) UsableConvoy.Payload ) )
				{
					byte[] b = await ISF.ReadAllBytes();
					await ISF.WriteBytes( Shared.Conv.Chinese.Translate( b ) );
				}
			}
			else if ( UsableConvoy.Payload is IStorageFile )
			{
				IStorageFile ISF = ( IStorageFile ) UsableConvoy.Payload;
				byte[] b = await ISF.ReadAllBytes();
				await ISF.WriteBytes( Shared.Conv.Chinese.Translate( b ) );
			}
			else if ( UsableConvoy.Payload is IEnumerable<string> )
			{
				List<string> Contents = new List<string>();
				foreach ( string Content in ( ( IEnumerable<string> ) UsableConvoy.Payload ) )
				{
					Contents.Add( _Translate( Content ) );
				}

				return new ProcConvoy( this, Contents );
			}
			else if ( UsableConvoy.Payload is string )
			{
				return new ProcConvoy( this, _Translate( ( string ) UsableConvoy.Payload ) );
			}

			return Convoy;
		}

		private string _Translate( string s )
		{
			if ( s == null )
				return s;
			return Shared.Conv.Chinese.Translate( s );
		}

	}
}