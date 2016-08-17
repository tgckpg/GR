using System.Threading.Tasks;
using Windows.Foundation;

namespace wenku8.Ext
{
    enum MemberStatus
    {
        LOGGED_IN
        , LOGGED_OUT
        , RE_LOGIN_NEEDED
    }

    interface IMember
    {
        MemberStatus Status { get; set; }

        bool IsLoggedIn { get; }
        bool WillLogin { get; }
        bool CanRegister { get; }

        string ServerMessage { get; }

        event TypedEventHandler<object, MemberStatus> OnStatusChanged;

        Task<bool> Register();
        void Logout();
        void Login( string name, string passwd );
    }

}