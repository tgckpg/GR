using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.UI.Converters;

namespace wenku8.Converters
{
    using Effects;

    sealed public class TransStateConverter : DataBoolConverter
    {
        public override object Convert( object value, Type targetType, object parameter, string language )
        {
            return DataBool( value, parameter != null ) ? TransitionState.Active : TransitionState.Inactive;
        }
    }
}