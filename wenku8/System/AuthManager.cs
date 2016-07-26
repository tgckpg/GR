using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

using Net.Astropenguin.IO;
using Net.Astropenguin.DataModel;
using Net.Astropenguin.Logging;

namespace wenku8.System
{
    using Settings;

    class AuthManager<T> : ActiveData
    {
        public enum KeyType : byte { AES = 1, RSA = 2 }
        public static readonly string ID = typeof( AuthManager<T> ).Name;

        protected string AuthKey;
        protected string AuthIncKey;
        protected string AuthName;

        private XRegistry AuthReg;

        public ObservableCollection<T> AuthList { get; private set; }
        public T SelectedItem { get; private set; }

        private XParameter XAuthInc;
        private int AuthInc = 0;

        public AuthManager( string AuthKey, string AuthIncKey, string AuthName )
        {
            this.AuthKey = AuthKey;
            this.AuthIncKey = AuthIncKey;
            this.AuthName = AuthName;

            AuthList = new ObservableCollection<T>();
            AuthReg = new XRegistry( "<keys />", FileLinks.ROOT_SETTING + FileLinks.SH_KEY_REG );

            XAuthInc = AuthReg.Parameter( AuthIncKey );

            if ( XAuthInc == null )
                XAuthInc = new XParameter( AuthIncKey );

            AuthInc = XAuthInc.GetSaveInt( "val" );

            // Read Keys
            XParameter[] Params = AuthReg.Parameters( AuthKey );

            foreach( XParameter P in Params )
            {
                AuthList.Add( CreateInstance( P ) );
            }

            AuthList.CollectionChanged += ( a, b ) => { NotifyChanged( "SelectedItem" ); };
            SelectedItem = AuthList.FirstOrDefault();
        }

        public T GetAuthById( string Id )
        {
            XParameter Param = AuthReg.Parameters( AuthKey )
                .FirstOrDefault( x => x.Parameter( Id ) != null );
            return Param == null ? default( T ) : CreateInstance( Param );
        }

        public void NewAuth()
        {
            XParameter NewKey = new XParameter( CryptAES.GenKey( 256 ) );
            NewKey.SetValue( new XKey[] {
                new XKey( AuthKey, "New " + AuthName + " " + AuthInc )
            } );

            XAuthInc.SetValue( new XKey( "val", AuthInc++ ) );
            AuthReg.SetParameter( NewKey );
            AuthReg.SetParameter( XAuthInc );
            AuthReg.Save();

            AuthList.Add( CreateInstance( NewKey ) );
            SelectedItem = AuthList.LastOrDefault();
        }

        public void AssignId( string Name, string Id )
        {
            XParameter Key = AuthReg.Parameters( AuthKey ).FirstOrDefault( x => x.GetValue( AuthKey ) == Name );

            if ( Key == null ) return;

            Key.SetParameter( new XParameter( Id ) );
            AuthReg.SetParameter( Key );
            AuthReg.Save();
        }

        virtual protected T CreateInstance( XParameter P )
        {
            return default( T );
        }
    }

    class AESManager : AuthManager<CryptAES>
    {
        public AESManager()
            :base( "aes", "aesinc", "SymKey" )
        {

        }

        public bool TryDecrypt( string EncData, out string Data )
        {
            foreach( CryptAES Crypts in AuthList )
            {
                try
                {
                    Data = Crypts.Decrypt( EncData );
                    return true;
                }
                catch ( Exception ) { }
            }

            Data = null;
            return false;
        }

        override protected CryptAES CreateInstance( XParameter P )
        {
            return new CryptAES( P.ID ) { Name = P.GetValue( AuthKey ) };
        }
    }

    class RSAManager : AuthManager<CryptRSA>
    {
        public RSAManager()
            : base( "rsa", "rsainc", "AsymKey" )
        {

        }

        override protected CryptRSA CreateInstance( XParameter P )
        {
            return new CryptRSA( "", P.ID ) { Name = P.GetValue( AuthKey ) };
        }
    }

    sealed class TokenManager : AuthManager<KeyValuePair<string, string>>
    {
        public TokenManager()
            : base( "tok", "tokinc", "Access Token" )
        {

        }

        protected override KeyValuePair<string, string> CreateInstance( XParameter P )
        {
            return new KeyValuePair<string, string>( P.GetValue( AuthKey ), P.ID );
        }
    }
}