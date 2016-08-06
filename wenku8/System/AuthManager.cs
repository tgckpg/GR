using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Helpers;
using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;

namespace wenku8.System
{
    using Settings;
    using Model.ListItem;

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

        protected AuthManager( string AuthKey, string AuthIncKey, string AuthName )
        {
            this.AuthKey = AuthKey;
            this.AuthIncKey = AuthIncKey;
            this.AuthName = AuthName;

            AuthList = new ObservableCollection<T>();
            AuthReg = new XRegistry( "<keys />", FileLinks.ROOT_AUTHMGR + AuthKey + ".xml" );

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

        public int ControlCount( string Id )
        {
            XParameter Param = AuthReg.Parameter( Id );
            return Param.Params.Count();
        }

        public void RenameAuth( string Old, string New )
        {
            XParameter Param = AuthReg.Parameters( AuthKey, Old ).FirstOrDefault();
            if( Param != null )
            {
                Param.SetValue( new XKey( AuthKey, New ) );
                AuthReg.SetParameter( Param );
                AuthReg.Save();
            }
        }

        public void RemoveAuth( string Id, T Item = default( T ) )
        {
            AuthReg.RemoveParameter( Id );
            AuthReg.Save();

            if( Item != null ) AuthList.Remove( Item );
        }

        public void NewAuth()
        {
            XParameter NewKey = new XParameter( CryptAES.GenKey( 256 ) );
            NewKey.SetValue( new XKey[] {
                new XKey( AuthKey, "New " + AuthName + " " + AuthInc )
            } );

            XAuthInc.SetValue( new XKey( "val", ++AuthInc ) );

            T Inst = CreateInstance( NewKey );

            AuthReg.SetParameter( NewKey );
            AuthReg.SetParameter( XAuthInc );
            AuthReg.Save();

            Worker.UIInvoke( () =>
            {
                AuthList.Add( Inst );
                SelectedItem = AuthList.LastOrDefault();
            } );
        }

        public void ImportAuth( string Name, string Auth )
        {
            XParameter ImpKey = AuthReg.Parameter( Auth );
            if ( ImpKey == null ) ImpKey = new XParameter( Auth );
            ImpKey.SetValue( new XKey( AuthKey, Name ) );

            AuthList.Add( CreateInstance( ImpKey ) );

            AuthReg.SetParameter( ImpKey );
            AuthReg.Save();
        }

        public void AssignId( string Name, string Id )
        {
            XParameter Key = AuthReg.Parameters( AuthKey ).FirstOrDefault( x => x.GetValue( AuthKey ) == Name );

            if ( Key == null ) return;

            Key.SetParameter( new XParameter( Id ) );
            AuthReg.SetParameter( Key );
            AuthReg.Save();
        }

        public void UnassignId( string Id )
        {
            XParameter[] Params = AuthReg.Parameters();

            foreach( XParameter Param in Params )
            {
                if( Param.Parameter( Id ) != null )
                {
                    Param.RemoveParameter( Id );
                    AuthReg.SetParameter( Param );
                }
            }

            AuthReg.Save();
        }

        virtual protected T CreateInstance( XParameter P )
        {
            return default( T );
        }
    }

    sealed class AESManager : AuthManager<CryptAES>
    {
        public AESManager() :base( "aes", "aesinc", "SymKey" ) { }

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
            return new CryptAES( P.Id ) { Name = P.GetValue( AuthKey ) };
        }
    }

    sealed class RSAManager : AuthManager<CryptRSA>
    {
        private RSAManager() : base( "rsa", "rsainc", "AsymKey" ) { }

        // This is because Generating / Reading Private key pairs are slow
        // TODO: Perhaps port the NTRUCrypt in the future and see if it is faster
        public static async Task<RSAManager> CreateAsync()
        {
            return await Task.Run( () => new RSAManager() );
        }

        [Obsolete( "You should use async instead because generating RSA keys are resources intensive", true )]
        new public void NewAuth() { }

        public async Task NewAuthAsync()
        {
            await Task.Run( () => { base.NewAuth(); } );
        }

        override protected CryptRSA CreateInstance( XParameter P )
        {
            CryptRSA RSA;
            if ( P.GetBool( "keypair" ) )
            {
                RSA = new CryptRSA( "", P.Id ) { Name = P.GetValue( AuthKey ) };
            }
            else
            {
                RSA = new CryptRSA() { Name = P.GetValue( AuthKey ) };
                P.SetValue( new XKey( "keypair", true ) );
                P.Id = RSA.GetPrivateKey();
            }

            return RSA;
        }
    }

    sealed class TokenManager : AuthManager<NameValue<string>>
    {
        public TokenManager() : base( "tok", "tokinc", "Access Token" ) { }

        protected override NameValue<string> CreateInstance( XParameter P )
        {
            return new NameValue<string>( P.GetValue( AuthKey ), P.Id );
        }
    }
}