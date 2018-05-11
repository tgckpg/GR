using Net.Astropenguin.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

using libtaotu.Controls;
using libtaotu.Models.Procedure;

using Net.Astropenguin.IO;
using Net.Astropenguin.Helpers;

namespace GR.Model.Book.Spider
{
	using Database.Models;
	using Interfaces;
	using Resources;
	using Settings;

	sealed class BookInstruction : BookItem, IInstructionSet
	{
		private ProcManager BSReference;
		public XParameter BookSpiderDef => BSReference?.ToXParam( GID );

		// Operation locks
		public static AsyncLocks<string, bool> OpLocks = new AsyncLocks<string, bool>();

		private SortedDictionary<int, ConvoyInstructionSet> Insts = new SortedDictionary<int, ConvoyInstructionSet>();

		public bool? Packed { get; private set; }
		public bool Packable => 0 < Insts.Count;

		public int LastIndex
		{
			get
			{
				if ( Insts.Count == 0 ) return 0;
				return Insts.Last().Key;
			}
		}

		public override bool NeedUpdate
		{
			get { return false; }
			protected set => base.NeedUpdate = value;
		}

		public BookInstruction( Book Bk ) : base( Bk ) { }
		public BookInstruction( string ZoneId, string ssid ) : base( ZoneId, BookType.S, ssid ) { }
		public BookInstruction() : this( null, Guid.NewGuid().ToString() ) { }

		// This will be set on Crawling
		public void PlaceDefs( string ssid, ProcManager BookSpider )
		{
			ZItemId = string.IsNullOrEmpty( ssid ) ? null : GSystem.Utils.Md5( ssid ).Substring( 0, 8 );
			BSReference = BookSpider;

			Entry.Meta[ AppKeys.GLOBAL_SSID ] = ssid;
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

		public override void Update( BookItem B )
		{
			base.Update( B );
			if( B is BookInstruction BInst )
			{
				Insts = BInst.Insts;
				BSReference = BInst.BSReference;
				Packed = BInst.Packed;
			}
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

			// We'll hash the TOC here. Which will be our identifier for the NeedUpdate variable
			CryptographicHash Hasher = HashAlgorithmProvider.OpenAlgorithm( HashAlgorithmNames.Sha1 ).CreateHash();

			foreach ( ConvoyInstructionSet Inst in Insts.Values )
			{
				Inst.SetConvoy( PPConvoy );

				if ( Inst is VolInstruction )
				{
					VInst = Inst as VolInstruction;
					Hasher.Append( Encoding.UTF8.GetBytes( VInst.Title ).AsBuffer() );
					continue;
				}

				if ( Inst is EpInstruction EInst )
				{
					VInst.PushInstruction( EInst );
					Hasher.Append( Encoding.UTF8.GetBytes( EInst.Title ).AsBuffer() );
				}
			}

			Entry.Meta[ "TOCHash" ] = CryptographicBuffer.EncodeToHexString( Hasher.GetValueAndReset() );

			if ( 0 < Ownerless.LastIndex )
			{
				PushInstruction( Ownerless );
			}

			Packed = true;
		}

		public async void PackSavedVols( XRegistry Settings )
		{
			if ( Packed != null ) return;

			if ( Entry.Volumes == null )
			{
				Entry.Volumes = await Shared.BooksDb.LoadCollectionAsync( Entry, x => x.Volumes, x => x.Index );
			}

			if ( !Volumes.Any() )
			{
				Packed = false;
				return;
			}

			foreach ( Volume Vol in Entry.Volumes )
			{
				VolInstruction VInst = new VolInstruction( Vol, Settings );

				if ( Vol.Chapters == null )
				{
					Vol.Chapters = await Shared.BooksDb.LoadCollectionAsync( Vol, x => x.Chapters, x => x.Index );
				}

				foreach ( Chapter Ch in Vol.Chapters )
				{
					VInst.PushInstruction( new EpInstruction( Ch, Settings ) );
				}

				PushInstruction( VInst );
			}

			Packed = 0 < Insts.Count;
		}

		public VolInstruction[] GetVolInsts()
		{
			return Insts.Values.Where( x => x is VolInstruction ).Cast<VolInstruction>().ToArray();
		}

	}
}