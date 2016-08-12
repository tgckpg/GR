using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.UI.Converters;

using wenku8.Effects;

namespace wenku8.Converters
{
    sealed public class TransStateConverter : DataBoolConverter
    {
        public override object Convert( object value, Type targetType, object parameter, string language )
        {
            return DataBool( value, parameter != null ) ? TransitionState.Active : TransitionState.Inactive;
        }
    }
}