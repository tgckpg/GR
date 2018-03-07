using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR.Model.Book.Spider;

namespace GR.Model.Interfaces
{
	interface IInstructionSet
	{
		void PushInstruction( IInstructionSet Inst );
		void Clear();
		int LastIndex { get; }
	}
}
