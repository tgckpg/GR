using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace wenku8.Converters
{
    sealed public class TOCTemplateSelector : DataTemplateSelector
    {
        public bool IsHorizontal = true;

        protected override DataTemplate SelectTemplateCore( object item, DependencyObject container )
        {
            FrameworkElement element = container as FrameworkElement;
            return element.FindName( IsHorizontal ? "Horizontal" : "Vertical" ) as DataTemplate;
        }
    }
}