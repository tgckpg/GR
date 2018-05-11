using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.IO;
using Net.Astropenguin.Linq;

using libtaotu.Controls;
using libtaotu.Models.Procedure;

using GR.Database.Models;
using GR.GSystem;
using GR.Model.Interfaces;
using GR.Settings;

namespace GR.Model.Book.Spider
{
	sealed class VolInstruction : ConvoyInstructionSet
	{
		public int Index { get; private set; }
		public string Title { get; private set; }

		public string VId => Utils.Md5( Title );
		public IReadOnlyList<IInstructionSet> EpInsts => SubInsts.AsReadOnly();

		public VolInstruction( int Index, string Title )
			:base()
		{
			this.Index = Index;
			this.Title = Title;
		}

		public VolInstruction( Volume Vol, XRegistry ProcDefs )
			: base()
		{
			Index = Vol.Index;
			Title = Vol.Title;
			ProcId = Vol.Meta[ "ProcId" ];

			Convoy = ProcParameter.RestoreParams( ProcDefs );

			XParameter ProcParam = ProcDefs.FindFirstMatch( "Guid", ProcId );
			if ( ProcParam != null )
			{
				ProcMan = new ProcManager();
				ProcMan.ReadParam( ProcParam );
			}

			for ( int i = 0; Vol.Meta.ContainsKey( "P" + i ); i++ )
			{
				PushConvoyParam( Vol.Meta[ "P" + i ] );
			}
		}

		public Volume ToVolume( Database.Models.Book Bk )
		{
			Volume Vol = new Volume()
			{
				Title = this.Title,
				Index = this.Index,
				BookId = Bk.Id,
				Chapters = SubInsts.Cast<EpInstruction>().Remap( x => x.ToChapter( Bk ) ).ToList()
			};

			Vol.Meta[ "ProcId" ] = ProcId;
			Vol.Meta[ AppKeys.GLOBAL_VID ] = VId;
			Vol.Chapters.ExecEach( x => x.Volume = Vol );
			ConvoyParams.ExecEach( ( x, i ) => Vol.Meta[ "P" + i ] = x );

			return Vol;
		}

		public override XParameter ToXParam()
		{
			throw new NotSupportedException();
		}
	}
}