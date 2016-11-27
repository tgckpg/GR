using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Ext
{
    using Model.ListItem;

    interface INavSelections
    {
        SubtleUpdateItem CustomSection();
        IMainPageSettings MainPage_Settings { get; }

        void Load();
    }
}