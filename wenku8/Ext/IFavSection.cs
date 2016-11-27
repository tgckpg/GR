using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Ext
{
    using Model.Interfaces;
    using Model.ListItem;

    interface IFavSection : ISearchableSection<FavItem>, ISectionItem, INotifyPropertyChanged
    {
        bool IsLoading { get; set; }
        FavItem CurrentItem { get; set; }

        void Reload();
        void Reload( bool Reset );
        void Load();
        void C_Pin();
        void C_RSync();
        void C_AutoCache();
        void C_Delete();
        void Reorder( int selectedIndex );
    }
}