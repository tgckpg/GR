using System;
using System.Numerics;

namespace wenku8.Effects.P2DFlow.ForceFields
{
    class GenericForce
    {
        public static GlobalForceField EARTH_GRAVITY = new GlobalForceField( new Vector2( 0, 9.8f ) );
    }
}
