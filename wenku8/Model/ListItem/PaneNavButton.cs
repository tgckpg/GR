using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Net.Astropenguin.DataModel;

namespace wenku8.Model.ListItem
{
	sealed class PaneNavButton : ActiveData
	{
		public FrameworkElement Icon { get; private set; }
		public Type Page { get; private set; }
		public Action Action { get; private set; }

		private bool _IsEnabled = true;
		public bool IsEnabled
		{
			get { return _IsEnabled; }
			set
			{
				_IsEnabled = value;
				NotifyChanged( "IsEnabled" );
			}
		}

		public PaneNavButton( FrameworkElement Icon, Type P )
		{
			this.Icon = Icon;
			Page = P;
		}

		public PaneNavButton( FrameworkElement Icon, Action A )
		{
			this.Icon = Icon;
			Action = A;
		}

		public void UpdateIcon( FrameworkElement Icon )
		{
			this.Icon = Icon;
			NotifyChanged( "Icon" );
		}
	}
}