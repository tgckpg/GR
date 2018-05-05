using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Graphics.Canvas;

namespace GR.Effects.P2DFlow.ForceFields
{
	class GlobalForceField : IForceField
	{
		public Vector2 a;

		public void Apply( Particle P )
		{
			P.a += a * P.gf;
		}

		public void WireFrame( CanvasDrawingSession ds ) { }
		public void FreeWireFrame() { }
	}
}