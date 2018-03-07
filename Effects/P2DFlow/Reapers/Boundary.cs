using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace GR.Effects.P2DFlow.Reapers
{
	class Boundary : IReaper
	{
		private Rect Bounds;

		public Boundary( Rect Bounds )
		{
			this.Bounds = Bounds;
		}

		public bool Reap( Particle p )
		{
			return !Bounds.Contains( p.Pos );
		}
	}
}
