using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI.Xaml.Media;

namespace wenku8.Effects.P2DFlow.Spawners
{
    class Trail : ISpawner
    {
        public float Chaos = 1.0f;

        public Trail() { }

        private int i;
        private Particle[] pp;

        public void Prepare( IEnumerable<Particle> part )
        {
            i = 0;
            pp = part.Where( p => ( p.Trait & PFTrait.TRAIL ) != 0 ).ToArray();
        }

        public int Acquire( int Quota )
        {
            return 2 * pp.Length;
        }

        public void Spawn( Particle P )
        {
            Particle OP = pp[ ( int ) Math.Floor( i ++ * 0.5 ) ];

            P.ttl = 30;

            P.a = Vector2.Transform( new Vector2( 10, 10 ), Matrix3x2.CreateRotation( 3.14f * Ext.RFloat() ) );
            P.Pos = OP.Pos;

            float ot = 100.0f + 5.0f * Ext.LFloat();
            P.vt = -Vector2.Normalize( P.v ) * ot;
        }
    }
}
