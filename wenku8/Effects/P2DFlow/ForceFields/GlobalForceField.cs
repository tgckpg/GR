using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Graphics.Canvas;

namespace wenku8.Effects.P2DFlow.ForceFields
{
    class GlobalForceField : IForceField
    {
        private Vector2 a;

        public GlobalForceField( Vector2 a )
        {
            this.a = a;
        }

        public void Apply( Particle P )
        {
            P.a += a * P.gf;
        }

        public void WireFrame( CanvasDrawingSession ds ) { }
        public void FreeWireFrame() { }
    }
}