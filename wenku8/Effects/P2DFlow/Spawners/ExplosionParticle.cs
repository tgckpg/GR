using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Effects.P2DFlow.Spawners
{
    class ExplosionParticle : ISpawner
    {
        public float Chaos = 1.0f;

        private int i;
        private Particle[] pp;

        public int Acquire( int Quota )
        {
            return pp.Count() * 60;
        }

        public void Prepare( IEnumerable<Particle> Ps )
        {
            i = 0;
            pp = Ps.Where( p => ( p.Trait & PFTrait.EXPLODE ) != 0 && p.ttl == 1 ).ToArray();
        }

        public void Spawn( Particle P )
        {
            Particle OP = pp[ ( int ) Math.Floor( i++ * 0.016 ) ];

            Vector2 XA = new Vector2( 30, 30 ) + 10 * Chaos * new Vector2( Ext.LFloat(), Ext.LFloat() );
            P.a = Vector2.Transform( XA, Matrix3x2.CreateRotation( 3.14f * Ext.RFloat() ) );
            P.Pos = OP.Pos;

            float ot = 60.0f + 40.0f * Ext.LFloat();
            P.vt = -Vector2.Normalize( P.v ) * ot;

            P.ttl = 40;

            P.Trait = PFTrait.FRAGMENT;
        }
    }
}
