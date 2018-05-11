using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using System.Numerics;

namespace GR.Effects.P2DFlow.ForceFields
{
	class Thrust : IForceField
	{
		public float StartTime = 0f;
		public float EndTime = 10f;
		public Vector2 Strength = new Vector2( 0, -10f );

		public void Apply( Particle P )
		{
			if ( ( P.Trait & PFTrait.THRUST ) != 0
				&& StartTime <= P.t && P.t <= EndTime )
			{
				P.a += Strength;
			}
		}

		public void WireFrame( CanvasDrawingSession ds ) { }
		public void FreeWireFrame() { }
	}
}