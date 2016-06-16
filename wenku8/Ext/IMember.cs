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
        bool IsLoggedIn { get; }
        bool WillLogin { get; }

        MemberStatus Status { get; set; }

        event TypedEventHandler<object, MemberStatus> OnStatusChanged;

        void Logout();
        void Login( string name, string passwd );
    }

}