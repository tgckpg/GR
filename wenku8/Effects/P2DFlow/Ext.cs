using System;
using System.Numerics;
using Windows.Foundation;

namespace wenku8.Effects.P2DFlow
{
    static class Ext 
    {
        public static bool Contains( this Rect R, Vector2 V )
        {
            return R.Contains( V.ToPoint() );
        }

        private static float CrossProduct( Vector2 A, Vector2 B, Vector2 C )
        {
            Vector2 AB = B - A;
            Vector2 AC = C - A;

            return AB.X * AC.Y - AB.Y * AC.X;
        }

        public static float DistanceTo( this Vector2 C, Vector2 A, Vector2 B, out Vector2 Projection )
        {
            float lengthSquared = Vector2.DistanceSquared( A, B );

            if ( lengthSquared == 0 )
            {
                Projection = A;
                return Vector2.Distance( C, A );
            }

            float t = Math.Max( 0, Math.Min( 1, Vector2.Dot( C - A, B - A ) / lengthSquared ) );
            Projection = A + t * ( B - A );

            return Vector2.Distance( C, Projection );
        }
    }
}