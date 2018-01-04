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
using GR.Settings;

namespace GR.Model.Book.Spider
{
	sealed class EpInstruction : ConvoyInstructionSet
	{
		public int Index { get; private set; }
		public string Title { get; private set; }

		public EpInstruction( int Index, string Title )
			:base()
		{
			this.Index = Index;
			this.Title = Title;
		}

		public EpInstruction( Chapter Ch, XRegistry ProcDefs )
			: base()
		{
			Index = Ch.Index;
			Title = Ch.Title;
			ProcId = Ch.Meta[ "ProcId" ];

			Convoy = ProcParameter.RestoreParams( ProcDefs );

			XParameter ProcParam = ProcDefs.FindFirstMatch( "Guid", ProcId );
			if ( ProcParam != null )
			{
				ProcMan = new ProcManager();
				ProcMan.ReadParam( ProcParam );
			}

			for ( int i = 0; Ch.Meta.ContainsKey( "P" + i ); i++ )
			{
				PushConvoyParam( Ch.Meta[ "P" + i ] );
			}
		}

		public Chapter ToChapter( Database.Models.Book Bk )
		{
			Chapter Ch = new Chapter()
			{
				Title = this.Title,
				Index = this.Index,
				BookId = Bk.Id,
			};

			Ch.Meta[ "ProcId" ] = ProcId;
			Ch.Meta[ AppKeys.GLOBAL_CID ] = Utils.Md5( Ch.Title );
			ConvoyParams.ExecEach( ( x, i ) => Ch.Meta[ "P" + i ] = x );

			return Ch;
		}

		public override XParameter ToXParam()
		{
			throw new NotSupportedException();
		}
	}
}