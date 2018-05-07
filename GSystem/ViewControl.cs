using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;

using Net.Astropenguin.DataModel;

namespace GR.GSystem
{
	sealed class ViewControl : ActiveData
	{
		private ApplicationView Instance;
		private DisplayInformation Disp;

		public ApplicationViewOrientation Orientation => Instance.Orientation;
		public DisplayOrientations DispOrientation => Disp.CurrentOrientation;

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

			Disp = DisplayInformation.GetForCurrentView();
		}

		public void ToggleFullScreen()
		{
			IsFullScreen = !Instance.IsFullScreenMode;
		}
	}
}