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
    sealed public class HSCTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Folded { get; set; }
        public DataTemplate Normal { get; set; }
        public DataTemplate Encrypted { get; set; }

        protected override DataTemplate SelectTemplateCore( object item, DependencyObject container )
        {
            HSComment HSC = ( HSComment ) item;

            if( HSC.Encrypted && HSC.DecFailed ) return Encrypted;
            return HSC.Folded ? Folded : Normal;
        }
    }
}