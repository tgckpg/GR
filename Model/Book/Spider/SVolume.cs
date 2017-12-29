using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using libtaotu.Models.Procedure;

namespace GR.Model.Book.Spider
{
	using Database.Models;

	sealed class SVolume
	{
		public VolInstruction Inst { get; private set; }

		public StorageFile TempFile { get; private set; }

		public SVolume( VolInstruction Inst, string vid, string aid, Chapter[] C )
		{
			throw new NotImplementedException();
		}

		public async Task SubProcRun( BookInstruction BInst )
		{
			IEnumerable<ProcConvoy> Convoys = await Inst.Process( BInst );
		}
	}
}