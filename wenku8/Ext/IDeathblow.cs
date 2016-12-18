using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Ext
{
    using Model.Book;
    using Model.ListItem;

    interface IDeathblow
    {
        string Id { get; }

        bool Registered { get; }
        bool Check( byte[] responseBytes );

        LocalBook GetParser();
        BookItem GetBook();

        void Register();
    }
}