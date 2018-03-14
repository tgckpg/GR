using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Config.Scopes
{
	interface IConf_BgContext
	{
		string BgType { get; set; }
		string BgValue { get; set; }
	}
}