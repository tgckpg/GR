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

namespace GR.Model.Book.Spider
{
	using Database.Models;
	using Interfaces;
	using ListItem;
	using Settings;

	sealed class BookInstruction : BookItem, IInstructionSet
	{
		private ProcManager BSReference;
		public XParameter BookSpiderDef
		{
			get { return BSReference.ToXParam( GID ); }
		}

		public string SId
		{
			get { return ZItemId; }
			private set
			{
				if ( string.IsNullOrEmpty( value ) )
				{
					ZItemId = value;
				}
				else
				{
					ZItemId = GSystem.Utils.Md5( value ).Substring( 0, 8 );
				}
			}
		}

		public override string VolumeRoot
		{
			get { return FileLinks.ROOT_SPIDER_VOL + ZoneId + "/" + ZItemId + "/"; }
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

		public BookInstruction( string Uid = null )
			: base( null, BookType.S, Uid ?? Guid.NewGuid().ToString() )
		{
			Insts = new SortedDictionary<int, ConvoyInstructionSet>();
		}

		public BookInstruction( string Uid, XRegistry Settings )
			: this( Uid )
		{
			ReadInfo( Settings );
		}

		public BookInstruction( string ZoneId, string ssid )
			: base( ZoneId, BookType.S, ssid )
		{
		}

		// This will be set on Selecting Zone Item
		public void SetId( string Id ) { this.ZoneId = Id; }

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

					throw new NotImplementedException();
					/*
					foreach ( Chapter C in Vol.Chapters )
					{
						SChapter SC = C as SChapter;
						if ( SC == null ) continue;

						XParameter CParam = SC.Inst.ToXParam();
						CParam.Id += j++;
						CParam.SetValue( new XKey( "EpInst", true ) );

						VParam.SetParameter( CParam );
					}
					*/

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
				.Remap( x => ( x as VolInstruction ).ToVolume( ZItemId ) )
				.Distinct( new VolDistinct() )
				.ToArray();
		}

		private class VolDistinct : IEqualityComparer<Volume>
		{
			public bool Equals( Volume x, Volume y )
			{
				return x.Id == y.Id;
			}

			public int GetHashCode( Volume obj )
			{
				return obj.Id.GetHashCode();
			}
		}
	}
}