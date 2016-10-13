using Microsoft.Graphics.Canvas.Effects;
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

        /// <summary>
        /// Gravity factor, how affected by gravity
        /// </summary>
        public float gf = 0.5f;

        /// <summary>
        /// Mass factor, how affected by forces such as Wind
        /// </summary>
        public float mf = 0.5f;

        /// <summary>
        /// Time to live
        /// </summary>
        public float ttl = 70;

        /// <summary>
        /// Minimum terminal velocity
        /// </summary>
        public float otMin = 100.0f;

        /// <summary>
        /// Maximum terminal velocity
        /// </summary>
        public float otMax = 165.0f;

        /// <summary>
        /// Spawning callback
        /// </summary>
        public Action<Particle> SpawnEx = ( P ) => { };

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

            P.gf = gf;
            P.mf = mf;
            P.ttl = ttl;

            float ot = otMin + ( otMax - otMin ) * NTimer.LFloat();
            P.vt = new Vector2( ot, ot );

            SpawnEx.Invoke( P );
        }

        public int Acquire( int Quota )
        {
            return ( i ++ ) % 10 == 0 ? 1 : 0;
        }
    }
}
