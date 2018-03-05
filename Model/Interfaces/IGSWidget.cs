using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Interfaces
{
	using Database.Models;

	interface IGSWidget
	{
		WidgetConfig DefaultWidgetConfig();
	}
}