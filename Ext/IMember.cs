using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;

namespace GR.Ext
{
	interface IMember : INotifyPropertyChanged
	{
		bool CanRegister { get; }
		bool IsLoggedIn { get; }

		string ServerMessage { get; }

		Task<bool> Register();
		Task<bool> Authenticate();
		Task<bool> Authenticate( string Account, string Password, bool Remember );

		void Logout();
	}
}