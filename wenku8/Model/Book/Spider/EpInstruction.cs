using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libtaotu.Controls;

using Net.Astropenguin.IO;

namespace wenku8.Model.Book.Spider
{
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

        public Chapter ToChapter( string aid, string vid )
        {
            return new SChapter( this, aid, vid );
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
