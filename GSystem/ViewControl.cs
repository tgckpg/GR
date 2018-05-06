using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

using Net.Astropenguin.DataModel;

namespace GR.GSystem
{
	sealed class ViewControl : ActiveData
	{
		private ApplicationView Instance;

		public ApplicationViewOrientation Orientation => Instance.Orientation;

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
					Instance.SetDesiredBoundsMode( ApplicationViewBoundsMode.UseCoreWindow );
				}
				else
				{
					Instance.ExitFullScreenMode();
					Instance.SetDesiredBoundsMode( ApplicationViewBoundsMode.UseVisible );
				}
				NotifyChanged( "IsFullScreen" );
			}
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