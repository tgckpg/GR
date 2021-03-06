﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Net.Astropenguin.IO;

using libtaotu.Controls;
using libtaotu.Models.Procedure;


namespace GR.Model.Book.Spider
{
	using Interfaces;

	/// <summary>
	/// ConvoyInstructionSet itself is a Procedure.
	/// It will be passed into the Spider as a Dispatcher with ProcType: INSTRUCTION
	/// </summary>
	class ConvoyInstructionSet : Procedure, IInstructionSet
	{
		protected List<string> ConvoyParams;
		public string ProcId { get; set; }

		public ProcManager ProcMan { get; set; }

		public Guid Id { get; set; }

		public int LastIndex { get { return SubInsts.Count; } }

		protected List<IInstructionSet> SubInsts;

		public ConvoyInstructionSet()
			:base( ProcType.INSTRUCTION )
		{
			Id = Guid.NewGuid();
			ConvoyParams = new List<string>();
			SubInsts = new List<IInstructionSet>();
		}

		virtual public void PushInstruction( IInstructionSet Inst )
		{
			SubInsts.Add( Inst );
		}

		virtual public void PushConvoyParam( string param )
		{
			ConvoyParams.Add( param );
		}

		virtual public void Clear()
		{
			ConvoyParams.Clear();
			SubInsts.Clear();
		}

		virtual public async Task<IEnumerable<ProcConvoy>> Process( object PassThru = null )
		{
			if( ProcMan != null )
			{
				List<ProcConvoy> Convoys = new List<ProcConvoy>();

				Procedure Proc = this;

				if( PassThru != null )
				{
					Proc = new ProcPassThru( new ProcConvoy( this, PassThru ) );
				}

				foreach( string Param in ConvoyParams )
				{
					Convoys.Add(
						await ProcMan.CreateSpider().Crawl( new ProcConvoy( Proc, Param ) )
					);
				}

				return Convoys;
			}

			return null;
		}

		public void SetConvoy( ProcConvoy Convoy )
		{
			this.Convoy = Convoy;
		}

		public void SetProcId( string Id )
		{
			ProcId = Id;
		}

		public override Task Edit()
		{
			throw new NotSupportedException();
		}
	}
}