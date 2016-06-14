using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.IO;

namespace wenku8.Ext
{
    interface ISectionItem
    {
        void Load( string ListName, bool useBookMeta = false );
        void Load( XKey[] Keys );
    }
}
