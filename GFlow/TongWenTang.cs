using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

using Net.Astropenguin.IO;
using Net.Astropenguin.Linq;
using Net.Astropenguin.Logging;

using GFlow.Controls;
using GFlow.Models.Interfaces;
using GFlow.Models.Procedure;

namespace GR.GFlow
{
	using Database.Schema;
	using Model.Book.Spider;
	using Resources;

	sealed class TongWenTang : Procedure
	{
		public TongWenTang()
			: base( ProcType.GENERIC )
		{
			RawName = "TRANSLATE";
		}

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

			if ( UsableConvoy == null )
				return Convoy;

			switch ( UsableConvoy.Payload )
			{
				case IEnumerable<IStorageFile> ISFs:
					foreach ( IStorageFile ISF in ISFs )
					{
						await ISF.WriteBytes( Shared.Conv.Chinese.Translate( await ISF.ReadAllBytes() ) );
					}
					break;
				case IStorageFile ISF:
					await ISF.WriteBytes( Shared.Conv.Chinese.Translate( await ISF.ReadAllBytes() ) );
					break;
				case IEnumerable<string> Texts:
					return new ProcConvoy( this, Texts.Remap( _Translate ) );
				case string Text:
					return new ProcConvoy( this, _Translate( Text ) );
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