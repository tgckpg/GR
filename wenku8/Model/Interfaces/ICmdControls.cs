using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace wenku8.Model.Interfaces
{
    interface ICmdControls
    {
        IList<ICommandBarElement> MajorControls { get; }
        IList<ICommandBarElement> MinorControls { get; }
    }
}