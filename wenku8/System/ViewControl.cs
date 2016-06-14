using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

using Net.Astropenguin.DataModel;

namespace wenku8.System
{

    class ViewControl : ActiveData
    {
        private ApplicationView Instance;
        public bool IsFullScreen
        {
            get
            {
                return Instance.IsFullScreenMode;
            }
            set
            {
                if ( value )
                {
                    Instance.TryEnterFullScreenMode();
                }
                else
                {
                    Instance.ExitFullScreenMode();
                }
                NotifyChanged( "IsFullScreen" );
            }
        }

        public ApplicationViewOrientation Orientation
        {
            get { return Instance.Orientation; }
        }

        public ViewControl()
        {
            Instance = ApplicationView.GetForCurrentView();
        }

        public void ToggleFullScreen()
        {
            IsFullScreen = !Instance.IsFullScreenMode;
        }
    }
}
