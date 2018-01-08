﻿using Net.Astropenguin.Linq;
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
	using Resources;
	using Settings;

	sealed class BookInstruction : BookItem, IInstructionSet
	{
		private ProcManager BSReference;
		public XParameter BookSpiderDef
		{
			get
			{
				if ( BSReference == null ) return null;
				return BSReference.ToXParam( GID );
			}
		}

		private SortedDictionary<int, ConvoyInstructionSet> Insts = new SortedDictionary<int, ConvoyInstructionSet>();
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

		public override bool NeedUpdate
		{
			get { return false; } // XXX: Check with TOC matching
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

			Shared.BooksDb.Entry( Entry ).Collection( x => x.Volumes ).Load();
			if ( !Volumes.Any() )
			{
				Packed = false;
				return;
			}

			foreach ( Volume Vol in Entry.Volumes )
			{
				VolInstruction VInst = new VolInstruction( Vol, Settings );

				Shared.BooksDb.Entry( Vol ).Collection( x => x.Chapters ).Load();

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