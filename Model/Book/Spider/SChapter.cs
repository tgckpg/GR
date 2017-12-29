using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using Net.Astropenguin.IO;

using libtaotu.Controls;
using libtaotu.Models.Procedure;

namespace GR.Model.Book.Spider
{
	using Database.Models;
	using Resources;
	using Settings;
	using GSystem;

	sealed class SChapter
	{
		private Chapter Ch;
		public EpInstruction Inst { get; private set; }
		public StorageFile TempFile { get; private set; }

		public SChapter( EpInstruction Inst, Chapter Ch )
		{
			Ch.Title = Inst.Title;
			this.Inst = Inst;
		}

		public async Task SubProcRun( bool useCache = true )
		{
			if ( useCache && !string.IsNullOrEmpty( Ch.Content.Text ) )
				return;

			IEnumerable<ProcConvoy> Convoys = await Inst.Process();

			TempFile = await AppStorage.MkTemp();

			foreach ( ProcConvoy Konvoi in Convoys )
			{
				ProcConvoy Convoy = ProcManager.TracePackage(
					Konvoi
					, ( d, c ) =>
					c.Payload is IEnumerable<IStorageFile>
					|| c.Payload is IStorageFile
				);

				if ( Convoy == null ) continue;

				if ( Convoy.Payload is IStorageFile )
				{
					await TempFile.WriteBytes(
						await ( Convoy.Payload as IStorageFile ).ReadAllBytes()
						, true, new byte[] { ( byte ) '\n' }
					);
					return;
				}

				IEnumerable<IStorageFile> ISFs = Convoy.Payload as IEnumerable<IStorageFile>;

				if ( ISFs == null || ISFs.Count() == 0 ) continue;

				foreach ( IStorageFile ISF in ISFs )
				{
					Shared.LoadMessage( "MergingContents", ISF.Name );
					await TempFile.WriteBytes( await ISF.ReadAllBytes(), true, new byte[] { ( byte ) '\n' } );
				}
			}
		}

	}
}