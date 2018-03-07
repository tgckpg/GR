using System;
using System.Threading.Tasks;

namespace GR.Model.Interfaces
{
	public interface IBackStackInterceptor
	{
		bool CanGoBack { get; }
		Action<object> Update_CanGoBack { get; set; }
		Task<bool> GoBack();
	}
}