using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.IO;
using Net.Astropenguin.Linq;

using libtaotu.Controls;
using libtaotu.Models.Procedure;

using wenku8.Model.Interfaces;

namespace wenku8.Model.Book.Spider
{
	using System;

	sealed class VolInstruction : ConvoyInstructionSet
	{
		public int Index { get; private set; }
		public string Title { get; private set; }

		public VolInstruction( int index, string title )
			:base()
		{
			this.Index = index;
			this.Title = title;
		}

		public VolInstruction( XParameter Param, XRegistry ProcDefs )
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

		public override void PushInstruction( IInstructionSet Inst )
		{
			base.PushInstruction( Inst );
		}

		public Volume ToVolume( string aid )
		{
			string id = Utils.Md5( this.Title );
			return new SVolume(
				this, id, aid
				, SubInsts
					.Cast<EpInstruction>()
					.Remap( x => x.ToChapter( aid, id ) )
					.ToArray()
			);
		}

		public override XParameter ToXParam()
		{
			XParameter Params = new XParameter( "VolInst" );
			Params.SetValue( new XKey( "ProcId", ProcId ) );
			Params.SetValue( new XKey( "Index", Index ) );
			Params.SetValue( new XKey( "Title", Title ) );
			Params.SetParameter( GetConvoyXParams() );
			return Params;
		}
	}
}