using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wenku8.Model.ListItem;

namespace wenku8.Ext
{
    interface IMainPageSettings
    {
        IEnumerable<SubtleUpdateItem> NavSections();
        ActiveItem SelectedSection { get; }
        bool IsStaffPicksEnabled { get; set; }
        bool IsCustomSectionEnabled { get; set; }

        Tuple<Type, string> PayloadCommand( string payload );
    }
}
