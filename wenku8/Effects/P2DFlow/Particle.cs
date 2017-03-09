using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;

namespace wenku8.Effects.P2DFlow
{
	class Particle
	{
		public Vector2 Pos;
		public float ttl = 0;
		public float t = 0;
		public int TextureId;

		// Velocity
		public Vector2 v;

		// Acceleration
		public Vector2 a;

		// Mass factor
		public float mf = 0;

		// Gravity factor
		public float gf = 0;

		// Terminal velocity
		public Vector2 vt;

		public Vector2 loss;

		public Matrix5x4 Tint;
		public Vector2 Scale;


		public Particle() { Reset(); }

		public PFTrait Trait = PFTrait.NONE;

		public void Reset()
		{
			gf = mf = ttl = t
				= Pos.X = Pos.Y
				= v.X = v.Y
				= a.X = a.Y = 0;

			TextureId = 0;

			Scale.X = Scale.Y = vt.X = vt.Y = 1;

			loss.X = loss.Y = 0.995f;
			Trait = PFTrait.NONE;

			Tint = new Matrix5x4()
			{
				M11 = 1, M12 = 0, M13 = 0, M14 = 0,
				M21 = 0, M22 = 1, M23 = 0, M24 = 0,
				M31 = 0, M32 = 0, M33 = 1, M34 = 0,
				M41 = 0, M42 = 0, M43 = 0, M44 = 1,
				M51 = 0, M52 = 0, M53 = 0, M54 = 0,
			};
		}
	}
} 