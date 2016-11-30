using Net.Astropenguin.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using libtaotu.Controls;
using libtaotu.Models.Procedure;

using Net.Astropenguin.IO;

namespace wenku8.Model.Book.Spider
{
    using Interfaces;
    using ListItem;
    using Settings;

    sealed class BookInstruction : BookItem, IInstructionSet
    {
        private string _ssid; // Save Sub Id

        // Id may either be:
        // 1. base.Id( Book Id )
        // 2. base.Id( Zone Id ) + _SSId( "/" + Id for Zone Item )
        public override string Id
        {
            get { return base.Id + _ssid; }
        }

        private ProcManager BSReference;
        public XParameter BookSpiderDef
        {
            get { return BSReference.ToXParam( Id ); }
        }


        private string _SId;
        public string SId
        {
            get { return _SId; }
            private set
            {
                _SId = value;
                if ( string.IsNullOrEmpty( value ) )
                {
                    _ssid = value;
                }
                else
                {
                    SetSSID( System.Utils.Md5( value ).Substring( 0, 8 ) );
                }
            }
        }

        public override string VolumeRoot
        {
            get { return FileLinks.ROOT_SPIDER_VOL + Id + "/"; }
        }

        private SortedDictionary<int, ConvoyInstructionSet> Insts;
        public bool? Packed { get; private set; }

        public bool Packable
        {
            get { return 0 < Insts.Count; }
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
            PackSavedVols( Sb.PSettings );
        }

        public BookInstruction( string GUID, XRegistry Settings )
            : this()
        {
            Id = GUID;
            ReadInfo( Settings );
        }

        public BookInstruction( string ZoneId, string ssid )
        {
            Id = ZoneId;
            SetSSID( ssid );
        }

        // This will be set on Selecting Zone Item
        public void SetId( string Id ) { this.Id = Id; }

        // This will create subdirectories for <Zone Id>/<ZoneItem Id>
        private void SetSSID( string Id ) { _ssid = "/" + Id; }

        // This will be set on Crawling
        public void PlaceDefs( string SId, ProcManager BookSpider )
        {
            this.SId = SId;
            this.BSReference = BookSpider;
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

        public void Clear()
        {
            Insts.Clear();
            Others.Clear();

            Packed = null;
        }

        public void PackVolumes( ProcConvoy PPConvoy )
        {
            if ( Packed == true ) return;
            // If VolInstructions were not present
            // All episodes will be pushed into this <Ownerless> Volume
            VolInstruction Ownerless = new VolInstruction( -1, string.IsNullOrEmpty( Title ) ? "Vol 1" : Title );
            VolInstruction VInst = Ownerless;

            foreach ( ConvoyInstructionSet Inst in Insts.Values )
            {
                Inst.SetConvoy( PPConvoy );

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

            if ( 0 < Ownerless.LastIndex )
            {
                PushInstruction( Ownerless );
            }

            Packed = true;
        }

        public void PackSavedVols( XRegistry Settings )
        {
            if ( Packed != null ) return;

            XRegistry XReg = new XRegistry( "<VolInfo />", TOCPath );
            XParameter[] VParams = XReg.Parameters( "VInst" );
            if ( VParams.Count() == 0 )
            {
                Packed = false;
                return;
            }

            foreach ( XParameter VParam in VParams )
            {
                VolInstruction VInst = new VolInstruction( VParam, Settings );

                IEnumerable<XParameter> CParams = VParam.Parameters( "EpInst" );
                foreach ( XParameter CParam in CParams )
                {
                    VInst.PushInstruction( new EpInstruction( CParam, Settings ) );
                }

                PushInstruction( VInst );
            }

            Packed = 0 < Insts.Count;
        }

        public async Task SaveTOC( IEnumerable<SVolume> SVols )
        {
            await Task.Run( () =>
            {
                XRegistry XReg = new XRegistry( "<VolInfo />", TOCPath, false );
                int i = 0;
                foreach ( SVolume Vol in SVols )
                {
                    XParameter VParam = Vol.Inst.ToXParam();
                    VParam.Id += i++;
                    VParam.SetValue( new XKey( "VInst", true ) );

                    int j = 0;
                    foreach ( Chapter C in Vol.ChapterList )
                    {
                        SChapter SC = C as SChapter;
                        if ( SC == null ) continue;

                        XParameter CParam = SC.Inst.ToXParam();
                        CParam.Id += j++;
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
            if ( Packed == null )
            {
                return new Volume[ 0 ];
            }

            return Insts.Values
                .Where( x => x is VolInstruction )
                .Remap( x => ( x as VolInstruction ).ToVolume( Id ) )
                .Distinct( new VolDistinct() )
                .ToArray();
        }

        public override void ReadInfo( XRegistry XReg )
        {
            base.ReadInfo( XReg );

            XParameter Param = XReg.Parameter( "METADATA" );

            SId = Param?.GetValue( "sid" );

            if ( !string.IsNullOrEmpty( _ssid ) )
            {
                // base.Id need to be chopped if sid present
                base.Id = base.Id.Replace( _ssid, "" );
            }
        }

        public override void SaveInfo( XRegistry XReg )
        {
            XParameter Param = XReg.Parameter( "METADATA" );
            if( Param == null ) Param = new XParameter( "METADATA" );

            Param.SetValue( new XKey( "sid", SId ) );
            XReg.SetParameter( Param );

            base.SaveInfo( XReg );
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