using System.Collections.Generic;

namespace GR.Effects.P2DFlow.Spawners
{
	interface ISpawner
	{
		void Prepare( IEnumerable<Particle> currParticles );
		int Acquire( int Quota );
		void Spawn( Particle p );
	}
}
