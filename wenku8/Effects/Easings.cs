using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Effects
{
    static class Easings
    {
        public static float OutQuintic( float t, float d )
        {
            if ( d <= t ) return 1;
            return ( ( t = t / d - 1 ) * t * t * t * t + 1 );
        }
    }
}