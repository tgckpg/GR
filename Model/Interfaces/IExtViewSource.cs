using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GR.Model.Interfaces
{
	abstract class PageExtension
	{
		protected Page Page { get; private set; }
		private volatile bool _Init = false;

		abstract protected void SetTemplate();
		abstract public void Unload();

		public void Extend( Page P )
		{
			Page = P;
			P.Loaded += Page_Loaded;
			P.Unloaded += Page_Unloaded;
			_SetTemplate();
		}

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
	}

	interface IExtViewSource
	{
		PageExtension Extension { get; }
	}
}