using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Interfaces
{
	interface IRoamable
	{
		bool CanRoam { get; }
		void Roam( double ox, double oy );
	}
}
