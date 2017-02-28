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
    using Resources;
    using Settings;
    using System;

    sealed class SChapter : Chapter
    {
        public EpInstruction Inst { get; private set; }

        protected override string VolRoot
        {
            get
            {
                return FileLinks.ROOT_SPIDER_VOL + aid + "/";
            }
        }

        public StorageFile TempFile { get; private set; }

        public SChapter( EpInstruction Inst, string aid, string vid )
            : base( Inst.Title, aid, vid, Utils.Md5( Inst.Title ) )
        {
            this.Inst = Inst;
        }

        public async Task SubProcRun( bool useCache = true )
        {
            if ( useCache && IsCached ) return;

            IEnumerable<ProcConvoy> Convoys = await Inst.Process();

            TempFile = await AppStorage.MkTemp();

            bool ToTrad = Config.Properties.LANGUAGE_TRADITIONAL;
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
                        ( await ( Convoy.Payload as IStorageFile ).ReadAllBytes() ).ToCTrad()
                        , true, new byte[] { ( byte ) '\n' }
                    );
                    return;
                }

                IEnumerable<IStorageFile> ISFs = Convoy.Payload as IEnumerable<IStorageFile>;

                if ( ISFs == null || ISFs.Count() == 0 ) continue;

                foreach ( IStorageFile ISF in ISFs )
                {
                    Shared.LoadMessage( "MergingContents", ISF.Name );
                    await TempFile.WriteBytes( ( await ISF.ReadAllBytes() ).ToCTrad(), true, new byte[] { ( byte ) '\n' } );
                }
            }
        }

    }
}