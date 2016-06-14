using Net.Astropenguin.Logging;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace wenku8.Converters
{
    public class ContentStyleConverter : IValueConverter
    {
        public static readonly string ID = typeof( ContentStyleConverter ).Name;

        public object Convert( object value, Type targetType, object parameter, string language )
        {
            // ToggleFav
            string AlignMode = value.ToString();

            if( Application.Current.Resources.ContainsKey( AlignMode ) )
            {
                Style S = ( Style ) Application.Current.Resources[ AlignMode ];
                return S;
            }
            else
            {
                // Try Get the assembly name
                Type P = Type.GetType( AlignMode );
                if( P != null )
                {
                    Style S = ( Style ) Application.Current.Resources[ P ];
                    return S;
                }
            }


            Logger.Log( ID, string.Format( "No such style \"{0}\"", AlignMode ), LogType.WARNING );
            return null;
        }

        public object ConvertBack( object value, Type targetType, object parameter, string language )
        {
            throw new NotImplementedException();
        }
    }
}
