using System.Collections.Generic;
using System.Numerics;

namespace GR.Effects.P2DFlow.ForceFields
{
	interface IForceField : IWireFrame
	{
		void Apply( Particle P );
	}
}
