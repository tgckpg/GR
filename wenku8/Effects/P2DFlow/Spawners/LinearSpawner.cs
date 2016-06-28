using System;
using System.Collections.Generic;
using System.Numerics;

namespace wenku8.Effects.P2DFlow.Spawners
{
    class LinearSpawner : ISpawner
    {
        public Vector2 Chaos = Vector2.One;
        public PFTrait SpawnTrait = PFTrait.NONE;
        public int Texture;

        private Vector2 Pos;
        private Vector2 Distrib;
        private Vector2 inVe;
        private int i = 0;

        public LinearSpawner() { }

        public LinearSpawner( Vector2 Pos, Vector2 Distrib, Vector2 InitVe )
        {
            this.Pos = Pos;
            this.Distrib = Distrib;
            inVe = InitVe;
        }

        public void Prepare( IEnumerable<Particle> currParticles ) { }

        public void Spawn( Particle P )
        {
            P.v = inVe - 2 * inVe * Chaos * new Vector2( NTimer.LFloat(), NTimer.LFloat() );
            P.Pos += Pos + Distrib * new Vector2( NTimer.RFloat(), NTimer.RFloat() );

            P.Trait = SpawnTrait;
            P.TextureId = Texture;

            P.gf = 0.5f;
            P.mf = 0.5f;
            P.ttl = 70;

            float ot = 100.0f + 65.0f * NTimer.LFloat();
            P.vt = new Vector2( ot, ot );
            P.Tint.M44 = 0;
        }

        public int Acquire( int Quota )
        {
            return ( i ++ ) % 10 == 0 ? 1 : 0;
        }
    }
}
