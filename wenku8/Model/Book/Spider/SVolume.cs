using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using Net.Astropenguin.IO;

using libtaotu.Controls;
using libtaotu.Models.Procedure;

namespace wenku8.Model.Book.Spider
{
    using System;

    class SVolume : Volume
    {
        public VolInstruction Inst { get; private set; }

        public StorageFile TempFile { get; private set; }

        public SVolume( VolInstruction Inst, string vid, string aid, Chapter[] C )
            :base( vid, false, Inst.Title, C )
        {
            this.Inst = Inst;
        }

        public async Task SubProcRun( BookInstruction BInst )
        {
            IEnumerable<ProcConvoy> Convoys = await Inst.Process( BInst );
        }
    }
}
