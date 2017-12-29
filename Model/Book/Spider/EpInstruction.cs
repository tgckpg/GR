using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libtaotu.Controls;
using libtaotu.Models.Procedure;

using Net.Astropenguin.IO;

namespace GR.Model.Book.Spider
{
	using Database.Models;

	sealed class EpInstruction : ConvoyInstructionSet
	{
		public int Index { get; private set; }
		public string Title { get; private set; }

		public EpInstruction( int index, string title )
			:base()
		{
			this.Index = index;
			this.Title = title;
		}

		public EpInstruction( XParameter Param, XRegistry ProcDefs )
			: base()
		{
			this.Index = Param.GetSaveInt( "Index" );
			this.Title = Param.GetValue( "Title" );
			this.ProcId = Param.GetValue( "ProcId" );

			Convoy = ProcParameter.RestoreParams( ProcDefs );

			XParameter ProcParam = ProcDefs.FindFirstMatch( "Guid", ProcId );
			if ( ProcParam != null )
			{
				ProcMan = new ProcManager();
				ProcMan.ReadParam( ProcParam );
			}

			foreach( XParameter ValParam in Param.Parameters( "Value" ) )
			{
				PushConvoyParam( ValParam.GetValue( "Value" ) );
			}
		}

		public Chapter ToChapter( Book Bk, Volume Vol )
		{
			return new Chapter()
			{
				Title = this.Title,
				BookId = Bk.Id,
				VolumeId = Vol.Id
			};
		}

		public override XParameter ToXParam()
		{
			XParameter Params = new XParameter( "EpInst" );
			Params.SetValue( new XKey( "ProcId", ProcId ) );
			Params.SetValue( new XKey( "Index", Index ) );
			Params.SetValue( new XKey( "Title", Title ) );
			Params.SetParameter( GetConvoyXParams() );
			return Params;
		}
	}
}