﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI.Xaml.Media;

namespace GR.Effects.P2DFlow.Spawners
{
	class Trail : ISpawner
	{
		public int Texture;
		public float Chaos = 1.0f;
		public float gf = 0;
		public float mf = 0;
		public Vector2 Scale = Vector2.One;

		public Trail() { }

		private int i;
		private Particle[] pp;

		public PFTrait Bind = PFTrait.TRAIL;

		public void Prepare( IEnumerable<Particle> part )
		{
			i = 0;
			pp = part.Where( p => ( p.Trait & Bind ) != 0 ).ToArray();
		}

		private float w = 0;

		public int Acquire( int Quota )
		{
			if ( w++ % 2 != 0 ) return 0;
			return 2 * pp.Length;
		}

		public void Spawn( Particle P )
		{
			Particle OP = pp[ ( int ) Math.Floor( i ++ * 0.5 ) ];

			P.TextureId = Texture;

			P.ttl = 30;

			P.a = Vector2.Transform( new Vector2( 10, 10 ), Matrix3x2.CreateRotation( 3.14f * NTimer.RFloat() ) );
			P.Pos = OP.Pos;
			P.mf = mf;
			P.gf = gf;
			P.Scale = Scale;

			float ot = 100.0f + 5.0f * NTimer.LFloat();
			P.vt = -Vector2.Normalize( P.v ) * ot;
		}
	}
}
