using Net.Astropenguin.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using Net.Astropenguin.IO;

using libtaotu.Controls;

namespace wenku8.Model.Book.Spider
{
    using Interfaces;
    using ListItem;
    using Settings;

    class BookInstruction : BookItem, IInstructionSet
    {
        public override string VolumeRoot
        {
            get { return FileLinks.ROOT_SPIDER_VOL + Id + "/"; }
        }

        private SortedDictionary<int, ConvoyInstructionSet> Insts;
        private bool Packed = false;

        public bool Packable
        {
            get { return Packed || 0 < Insts.Count; }
        }

        public int LastIndex
        {
            get
            {
                if ( Insts.Count == 0 ) return 0;
                return Insts.Last().Key;
            }
        }

        public BookInstruction()
        {
            Insts = new SortedDictionary<int, ConvoyInstructionSet>();
            Id = Guid.NewGuid().ToString();
        }

        public BookInstruction( SChapter C )
            : this()
        {
            Id = C.aid;
            SpiderBook Sb = new SpiderBook( this );
            ReadInfo( Sb.PSettings );
            TryReadTOC( Sb.PSettings );
        }

        public BookInstruction( string GUID, XRegistry Settings )
            : this()
        {
            Id = GUID;
            ReadInfo( Settings );
            TryReadTOC( Settings );
        }

        public void PushInstruction( IInstructionSet Inst )
        {
            if ( Inst is VolInstruction )
            {
                PushVolume( Inst as VolInstruction );
            }
            else if ( Inst is EpInstruction )
            {
                PushChapter( Inst as EpInstruction );
            }
        }

        private void PushVolume( VolInstruction Vol )
        {
            int i = Vol.Index;
            while ( Insts.ContainsKey( i ) ) i++;
            Insts.Add( i, Vol );
        }

        private void PushChapter( EpInstruction Ep )
        {
            int i = Ep.Index;
            while ( Insts.ContainsKey( i ) ) i++;
            Insts.Add( i, Ep );
        }

        public void PackVolumes()
        {
            if ( Packed ) return;
            VolInstruction Ownerless = new VolInstruction( -1, "<Ownerless>" );
            VolInstruction VInst = Ownerless;
            foreach ( ConvoyInstructionSet Inst in Insts.Values )
            {
                if ( Inst is VolInstruction )
                {
                    VInst = Inst as VolInstruction;
                    continue;
                }

                if ( Inst is EpInstruction )
                {
                    VInst.PushInstruction( Inst as EpInstruction );
                }
            }

            if( 0 < Ownerless.LastIndex )
            {
                PushInstruction( Ownerless );
            }

            Packed = true;
        }

        protected void TryReadTOC( XRegistry Settings )
        {
            XRegistry XReg = new XRegistry( "<VolInfo />", TOCPath );
            XParameter[] VParams = XReg.GetParametersWithKey( "VInst" );
            if ( VParams.Count() == 0 ) return;

            foreach ( XParameter VParam in VParams )
            {
                VolInstruction VInst = new VolInstruction( VParam, Settings );

                IEnumerable<XParameter> CParams = VParam.GetParametersWithKey( "EpInst" );
                foreach ( XParameter CParam in CParams )
                {
                    VInst.PushInstruction( new EpInstruction( CParam, Settings ) );
                }

                PushInstruction( VInst );
            }

            Packed = true;
        }

        public async Task CompileTOC( IEnumerable<SVolume> SVols )
        {
            await Task.Run( () =>
            {
                XRegistry XReg = new XRegistry( "<VolInfo />", TOCPath, false );
                int i = 0;
                foreach ( SVolume Vol in SVols )
                {
                    XParameter VParam = Vol.Inst.ToXParam();
                    VParam.ID += i++;
                    VParam.SetValue( new XKey( "VInst", true ) );

                    int j = 0;
                    foreach ( Chapter C in Vol.ChapterList )
                    {
                        SChapter SC = C as SChapter;
                        if ( SC == null ) continue;

                        XParameter CParam =  SC.Inst.ToXParam();
                        CParam.ID += j++;
                        CParam.SetValue( new XKey( "EpInst", true ) );

                        VParam.SetParameter( CParam );
                    }

                    XReg.SetParameter( VParam );
                }

                XReg.Save();
            } );
        }

        public override Volume[] GetVolumes()
        {
            if ( !Packed ) throw new Exception( "Unpacked Book instruction" );

            return Insts.Values
                .Where( x => x is VolInstruction )
                .Remap( x => ( x as VolInstruction ).ToVolume( Id ) )
                .Distinct( new VolDistinct ())
                .ToArray();
        }

        private class VolDistinct : IEqualityComparer<Volume>
        {
            public bool Equals( Volume x, Volume y )
            {
                return x.vid == y.vid;
            }

            public int GetHashCode( Volume obj )
            {
                return obj.vid.GetHashCode();
            }
        }
    }
}
