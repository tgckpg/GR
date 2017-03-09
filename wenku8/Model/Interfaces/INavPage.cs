using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Model.Interfaces
{
	interface INavPage
	{
		// Used for Re-entering page
		void SoftOpen();

		// Used for Page navigated out
		void SoftClose();
	}
}