using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Interfaces
{
	public interface IBookProcess
	{
		bool CanProcess { get; }
		bool Processed { get; }
		bool ProcessSuccess { get; }
		bool Processing { get; }

		string Zone { get; }
		string Name { get; }
		string Desc { get; }
		string Desc2 { get; }
	}
}