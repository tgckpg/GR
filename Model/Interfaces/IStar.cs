﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Interfaces
{
	interface IStar : IRoamable
	{
		bool Reacting { get; }

		Task Explode();
		Task Vanquish();
	}
}
