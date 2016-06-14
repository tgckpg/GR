﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Ext
{
    interface INavSelections
    {
        IPaneInfoSection CustomSection();
        IMainPageSettings MainPage_Settings { get; }

        void Load();
    }
}
