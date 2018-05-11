using System;
using System.Numerics;

namespace GR.Effects.P2DFlow.ForceFields
{
	class GenericForce
	{
		public static GlobalForceField EARTH_GRAVITY = new GlobalForceField() { a = new Vector2( 0, 9.8f ) };
	}
}
