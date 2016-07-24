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
    using AdvDM;
    using Model.REST;
    using Resources;
    using Settings;

    sealed class AuthManager : ActiveData
    {
        public static readonly string ID = typeof( AuthManager ).Name;

        private XRegistry AuthReg;

        public ObservableCollection<CryptAES> KeyList { get; private set; }
        public ObservableCollection<KeyValuePair<string, string>> TokList { get; private set; }
        public CryptAES SelectedKey { get; private set; }
        public KeyValuePair<string, string> SelectedToken { get; private set; }

        private XParameter XKeyInc;
        private XParameter XTokInc;
        private int KeyInc = 0;
        private int TokInc = 0;

        public AuthManager()
        {
            KeyList = new ObservableCollection<CryptAES>();
            TokList = new ObservableCollection<KeyValuePair<string, string>>();
            AuthReg = new XRegistry( "<keys />", FileLinks.ROOT_SETTING + FileLinks.SH_KEY_REG );

            XKeyInc = AuthReg.GetParameter( "keyinc" );

            if ( XKeyInc == null )
                XKeyInc = new XParameter( "keyinc" );

            KeyInc = XKeyInc.GetSaveInt( "val" );

            XTokInc = AuthReg.GetParameter( "keyinc" );

            if ( XTokInc == null )
                XTokInc = new XParameter( "tokinc" );

            TokInc = XTokInc.GetSaveInt( "val" );

            // Read Keys
            XParameter[] Params = AuthReg.GetParametersWithKey( AppKeys.AM_KEY );
            foreach( XParameter P in Params )
            {
                KeyList.Add( new CryptAES( P.ID ) { Name = P.GetValue( AppKeys.AM_KEY ) } );
            }

            KeyList.CollectionChanged += ( a, b ) => { NotifyChanged( "SelectedKey" ); };
            SelectedKey = KeyList.FirstOrDefault();

            // Read Tokens
            Params = AuthReg.GetParametersWithKey( AppKeys.AM_TOK );
            foreach( XParameter P in Params )
            {
                TokList.Add( new KeyValuePair<string, string>( P.GetValue( AppKeys.AM_TOK ), P.ID ) );
            }

            TokList.CollectionChanged += ( a, b ) => { NotifyChanged( "SelectedToken" ); };
            SelectedToken = TokList.FirstOrDefault();
        }

        public string GetATokenById( string Id )
        {
            XParameter Param = AuthReg.GetParametersWithKey( AppKeys.AM_TOK )
                .FirstOrDefault( x => x.GetParameter( Id ) != null );
            return Param == null ? "" : Param.ID;
        }

        public CryptAES GetKeyById( string Id )
        {
            XParameter Param = AuthReg.GetParametersWithKey( AppKeys.AM_KEY )
                .FirstOrDefault( x => x.GetParameter( Id ) != null );
            return Param == null ? null : new CryptAES( Param.ID );
        }

        public bool TryDecrypt( string EncData, out string Data )
        {
            foreach( CryptAES Crypts in KeyList )
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

        public void NewKey()
        {
            string Key = CryptAES.GenKey( 256 );

            XParameter NewKey = new XParameter( Key );
            NewKey.SetValue( new XKey( AppKeys.AM_KEY, "New Key " + KeyInc ) );

            XKeyInc.SetValue( new XKey( "val", KeyInc + 1 ) );
            AuthReg.SetParameter( NewKey );
            AuthReg.SetParameter( XKeyInc );
            AuthReg.Save();

            KeyList.Add( new CryptAES( Key ) { Name = "New Key " + ( KeyInc++ ) } );
            SelectedKey = KeyList.LastOrDefault();
        }

        public void NewAccessToken()
        {
            string Token = CryptAES.GenKey( 32 );

            XParameter NewToken = new XParameter( Token );
            NewToken.SetValue( new XKey( AppKeys.AM_TOK, "New AccessToken " + TokInc ) );

            XTokInc.SetValue( new XKey( "val", TokInc + 1 ) );
            AuthReg.SetParameter( NewToken );
            AuthReg.SetParameter( XTokInc );
            AuthReg.Save();

            TokList.Add( new KeyValuePair<string, string>( "New AccessToken " + ( TokInc++ ), Token ) );
            SelectedToken = TokList.LastOrDefault();
        }

        public async Task<string> ReserveId( string AccessToken )
        {
            TaskCompletionSource<string> TCS = new TaskCompletionSource<string>();

            RuntimeCache RCache = new RuntimeCache();
            RCache.POST(
                Shared.ShRequest.Server
                , Shared.ShRequest.ReserveId( AccessToken )
                , ( e, QueryId ) =>
                {
                    try
                    {
                        JsonObject JDef = JsonStatus.Parse( e.ResponseString );
                        string Id = JDef.GetNamedString( "data" );
                        TCS.SetResult( Id );
                    }
                    catch( Exception ex )
                    {
                        Logger.Log( ID, ex.Message, LogType.WARNING );
                        TCS.SetCanceled();
                    }
                }
                , ( cache, Id, ex ) =>
                {
                    Logger.Log( ID, ex.Message, LogType.WARNING );
                    TCS.SetCanceled();
                }
                , false
            );

            return await TCS.Task;
        }

        public void AssignTokenId( string Name, string Id )
        {
            AssignId( AppKeys.AM_TOK, Name, Id );
        }

        public void AssignKeyId( string Name, string Id )
        {
            AssignId( AppKeys.AM_KEY, Name, Id );
        }

        private void AssignId( string KName, string Name, string Id )
        {
            XParameter Key = AuthReg.GetParametersWithKey( KName ).FirstOrDefault( x => x.GetValue( KName ) == Name );

            if( Key != null )
            {
                Key.SetParameter( new XParameter( Id ) );
                AuthReg.SetParameter( Key );
            }

            AuthReg.Save();
        }
    }
}