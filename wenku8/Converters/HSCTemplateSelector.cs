using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace wenku8.Converters
{
    using Model.Comments;
    public class HSCTemplateSelector : DataTemplateSelector
    {
        public bool IsHorizontal = true;

        public DataTemplate Folded { get; set; }
        public DataTemplate Normal { get; set; }

        protected override DataTemplate SelectTemplateCore( object item, DependencyObject container )
        {
            return ( ( HSComment ) item ).Folded ? Folded : Normal;
        }
    }
}
