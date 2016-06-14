using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wenku8.Model.Book.Spider;

namespace wenku8.Model.Interfaces
{
    interface IInstructionSet
    {
        void PushInstruction( IInstructionSet Inst );
        int LastIndex { get; }
    }
}
