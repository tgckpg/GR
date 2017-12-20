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

using libtaotu.Controls;
using libtaotu.Models.Procedure;

namespace GR.Taotu
{
	using CompositeElement;
	using Model.Book.Spider;
	using Resources;

	sealed class TongWenTang : Procedure
	{
		protected override IconBase Icon { get { return new IconInfo() { AutoScale = true }; } }
		protected override Color BgColor { get { return Colors.Gray; } }

		public TongWenTang() : base( ProcType.TRANSLATE ) { }

		public override async Task<ProcConvoy> Run( ProcConvoy Convoy )
		{
			if ( !Shared.TC.DoTranslate ) return Convoy;

			// Book Instruction Translate
			if ( Convoy.Dispatcher is GrimoireExtractor && Convoy.Payload is BookInstruction )
			{
				BookInstruction Book = ( BookInstruction ) Convoy.Payload;

				// Read Param
				XRegistry InfoReg = new XRegistry( "<wentang />", null, false );
				Book.SaveInfo( InfoReg );

				// Translate and Read
				InfoReg = new XRegistry( Shared.TC.Translate( InfoReg.ToString() ), null, false );
				Book.ReadInfo( InfoReg );

				return Convoy;
			}

			ProcManager.PanelMessage( this, Res.RSTR( "IncomingCheck" ), LogType.INFO );

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
					await ISF.WriteBytes( Shared.TC.Translate( b ) );
				}
			}
			else if ( UsableConvoy.Payload is IStorageFile )
			{
				IStorageFile ISF = ( IStorageFile ) UsableConvoy.Payload;
				byte[] b = await ISF.ReadAllBytes();
				await ISF.WriteBytes( Shared.TC.Translate( b ) );
			}
			else if ( UsableConvoy.Payload is IEnumerable<string> )
			{
				List<string> Contents = new List<string>();
				foreach ( string Content in ( ( IEnumerable<string> ) UsableConvoy.Payload ) )
				{
					Contents.Add( Shared.TC.Translate( Content ) );
				}

				return new ProcConvoy( this, Contents );
			}
			else if ( UsableConvoy.Payload is string )
			{
				return new ProcConvoy( this, Shared.TC.Translate( ( string ) UsableConvoy.Payload ) );
			}

			return Convoy;
		}

		public override Task Edit()
		{
			StringResources stx = new StringResources( "Message" );
			return Popups.ShowDialog( UIAliases.CreateDialog( stx.Str( "ProcNoOptions" ) ) );
		}
	}
}