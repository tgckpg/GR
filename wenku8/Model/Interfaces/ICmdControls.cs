using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace wenku8.Model.Interfaces
{
    public delegate void ControlChangedEvent( object sender );

    interface ICmdControls
    {
        event ControlChangedEvent ControlChanged;

        bool NoCommands { get; }
        bool MajorNav { get; }

        IList<ICommandBarElement> MajorControls { get; }
        IList<ICommandBarElement> Major2ndControls { get; }
        IList<ICommandBarElement> MinorControls { get; }
    }
}