using System.Collections.Generic;
using System.Numerics;

namespace wenku8.Effects.P2DFlow.ForceFields
{
    interface IForceField : IWireFrame
    {
        void Apply( Particle P );
    }
}
