using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace GR.Model.Interfaces
{
	abstract class PageExtension
	{
		protected Page Page { get; private set; }
		private volatile bool _Init = false;

		abstract protected void SetTemplate();
		abstract public void Unload();

		public void Initialize( Page P )
		{
			Page = P;
			Extend( P );

			P.Loaded += Page_Loaded;
			P.Unloaded += Page_Unloaded;
			_SetTemplate();
		}

		virtual protected void Extend( Page P ) { }

		private void _SetTemplate()
		{
			if ( _Init ) return;
			_Init = true;

			SetTemplate();
		}

		private void _Unload()
		{
			if ( !_Init ) return;
			Unload();
		}

		private void Page_Loaded( object sender, RoutedEventArgs e ) => _SetTemplate();
		private void Page_Unloaded( object sender, RoutedEventArgs e ) => _Unload();

		virtual public FlyoutBase GetContextMenu( FrameworkElement Elem )
		{
			return null;
		}
	}

	interface IExtViewSource
	{
		PageExtension Extension { get; }
	}
}