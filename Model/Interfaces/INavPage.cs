﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Interfaces
{
	interface INavPage
	{
		// Used for Re-entering page
		void SoftOpen( bool NavForward );

		// Used for Page navigated out
		void SoftClose( bool NavForward );
	}
}