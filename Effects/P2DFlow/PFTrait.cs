using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Effects.P2DFlow
{
	enum PFTrait : uint
	{
		NONE = 0,
		IMMORTAL = 1,
		TRAIL = 2,
		TRAIL_O = 4,
		EXPLODE = 8,
		FRAGMENT = 16,
		THRUST = 32,
	}
}