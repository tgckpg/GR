using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;

using Net.Astropenguin.Helpers;

namespace wenku8.Effects.P2DFlow.Spawners
{
	class PointerSpawner : ISpawner
	{
		public float Chaos = 1.0f;
		public int Texture;

		private int i;

		public PFTrait SpawnTrait = PFTrait.NONE;

		private Vector2 DrawPos;
		private int Draw = 0;

		public void FeedPosition( Vector2 P )
		{
			DrawPos = P;
			Draw = 1;
		}

		public PointerSpawner() { }

		public int Acquire( int Quota )
		{
			return Draw;
		}

		public void Prepare( IEnumerable<Particle> Ps )
		{
		}

		public void Spawn( Particle P )
		{
			P.Trait = SpawnTrait;
			P.TextureId = Texture;

			P.ttl = 2;
			P.Pos = DrawPos;

			Draw = 0;
		}
	}
}
