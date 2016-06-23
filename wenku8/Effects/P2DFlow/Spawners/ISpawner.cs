using System.Collections.Generic;

namespace wenku8.Effects.P2DFlow.Spawners
{
    interface ISpawner
    {
        void Prepare( IEnumerable<Particle> currParticles );
        int Acquire( int Quota );
        void Spawn( Particle p );
    }
}
