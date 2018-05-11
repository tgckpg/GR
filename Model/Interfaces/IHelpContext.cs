using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Interfaces
{
	public interface IHelpContext : INotifyPropertyChanged
	{
		bool Help_Show { get; }
		string Help_Title { get; }
		string Help_Desc { get; }
		Uri Help_Uri { get; }
	}
}