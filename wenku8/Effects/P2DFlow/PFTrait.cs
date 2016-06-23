using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Effects.P2DFlow
{
    enum PFTrait : uint
    {
        NONE = 0,
        IMMORTAL = 1,
        TRAIL = 2,
        EXPLODE = 4,
        FRAGMENT = 8,
        THRUST = 16,
    }
}