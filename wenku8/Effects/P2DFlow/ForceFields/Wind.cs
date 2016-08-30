using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Windows.UI;

namespace wenku8.Effects.P2DFlow.ForceFields
{
    class Wind : IForceField
    {
        public float Strength = 50f;
        public float MaxDist = 200.0f;

        public Vector2 A;
        public Vector2 B;

#if DEBUG
        private List<Tuple<Vector2, Vector2, float>> DebugInfo = new List<Tuple<Vector2, Vector2, float>>();
#endif

        virtual public void Apply( Particle P )
        {
            Vector2 C;
            float dist = P.Pos.DistanceTo( A, B, out C );

            if ( MaxDist < dist ) return;

            float Gradient =  P.mf * ( 1 - dist / MaxDist );

            Vector2 NormG = Vector2.Normalize( P.Pos - C ) * Gradient;
            P.a += NormG * Strength;

#if DEBUG
            DebugInfo.Add( new Tuple<Vector2, Vector2, float>( C, P.Pos, Gradient ) );
#endif
        }

        public void WireFrame( CanvasDrawingSession ds )
        {
            ds.DrawLine( A, B, Colors.White );
            ds.DrawCircle( A, 10, Colors.DarkCyan );
            ds.DrawCircle( B, 10, Colors.DarkCyan );

#if DEBUG
            foreach( Tuple<Vector2, Vector2, float> d in DebugInfo )
            {
                ds.DrawLine( d.Item1, d.Item2, Color.FromArgb( ( byte ) Math.Floor( 255 * d.Item3 ), 0, 255, 0 ) );
            }

            DebugInfo = new List<Tuple<Vector2, Vector2, float>>();
#endif
        }
    }
}