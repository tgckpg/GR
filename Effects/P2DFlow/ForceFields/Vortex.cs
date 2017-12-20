using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

using Microsoft.Graphics.Canvas;

namespace GR.Effects.P2DFlow.ForceFields
{
	enum VortexSpin { Clockwise, Anticlockwise }

	class Vortex : IForceField
	{
		public float Strength = 50f;
		public float MaxDist = 100.0f;

		public Vector2 Center;
		public float Eye = 50.0f;
		public VortexSpin Direction;

#if DEBUG
		private List<Tuple<Vector2, Vector2, float>> DebugInfo = new List<Tuple<Vector2, Vector2, float>>();
#endif

		private float ForceLine = 0;
		private float ForceLineO = 0;

		public Vortex( float Eye, float MaxDist )
		{
			this.Eye = Eye;
			this.MaxDist = MaxDist;

			ForceLineO = 0.5f * ( MaxDist - Eye );
			ForceLine = Eye + ForceLineO;
		}

		public void Apply( Particle P )
		{
			float dist = Vector2.Distance( P.Pos, Center );

			if ( MaxDist < dist || dist < Eye ) return;

			dist -= Eye;
			float Gradient = Math.Abs( P.mf * ( dist - ForceLine ) / ForceLineO );

			Vector2 NormG = VSpin( Vector2.Normalize( P.Pos - Center ) ) * Gradient;
			P.a += NormG * Strength;

#if DEBUG
			DebugInfo.Add( new Tuple<Vector2, Vector2, float>( Center, P.Pos, Gradient ) );
#endif
		}

		private Vector2 VSpin( Vector2 V )
		{
			if ( Direction == VortexSpin.Clockwise )
				return new Vector2( -V.Y, V.X );
			else
				return new Vector2( V.Y, -V.X );
		}

		public void WireFrame( CanvasDrawingSession ds )
		{
			ds.DrawCircle( Center, Eye, Colors.DarkCyan );
			ds.DrawCircle( Center, ForceLine, Colors.Red );
			ds.DrawCircle( Center, MaxDist, Colors.DarkCyan );

#if DEBUG
			foreach( Tuple<Vector2, Vector2, float> d in DebugInfo )
			{
				ds.DrawLine( d.Item1, d.Item2, Color.FromArgb( ( byte ) Math.Floor( 255 * d.Item3 ), 0, 255, 0 ) );
			}
#endif
		}

		public void FreeWireFrame()
		{
#if DEBUG
			DebugInfo = new List<Tuple<Vector2, Vector2, float>>();
#endif
		}

	}
}