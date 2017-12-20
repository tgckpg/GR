using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GR.Converters
{
	using Model.ListItem.Sharers;

	sealed public class SHRequestTemplateSelector : DataTemplateSelector
	{
		public DataTemplate Removed { get; set; }
		public DataTemplate Normal { get; set; }

		protected override DataTemplate SelectTemplateCore( object item, DependencyObject container )
		{
			SHGrant HSC =  ( ( GrantProcess ) item ).GrantDef;
			return HSC.SourceRemoved ? Removed : Normal;
		}
	}
}