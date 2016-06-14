namespace wenku8.Ext
{
    delegate void StatusUpdate();

    interface IMember
    {
        bool IsLoggedIn { get; }
        bool WillLogin { get; }

		event StatusUpdate OnStatusChanged;

        void Logout();
        void Login( string name, string passwd );
    }
}