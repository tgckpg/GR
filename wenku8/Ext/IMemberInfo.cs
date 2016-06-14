using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Ext
{
    interface IMemberInfo
    {
        event PropertyChangedEventHandler PropertyChanged;
        string Signature { get; set; }
    }
}
