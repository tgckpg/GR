using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;

namespace wenku8.Model.ListItem.Sharers
{
    using AdvDM;
    using REST;
    using Resources;
    using System;

    using SHTarget = REST.SharersRequest.SHTarget;

    sealed class GrantProcess : ActiveData
    {
        public static readonly string ID = typeof( GrantProcess ).Name;

        public string ScriptId { get { return GrantDef.ScriptId; } }
        public int NGrants { get { return GrantDef.Grants.Length; } }
        public SHTarget Target { get { return GrantDef.Target; } }

        public SHGrant GrantDef { get; private set; }

        #region Display Properties
        private string _Title;
        public string Title
        {
            get { return _Title; }
            private set
            {
                _Title = value;
                NotifyChanged( "Title" );
            }
        }

        private bool _Processed = false;
        public bool Processed
        {
            get { return _Processed; }
            private set
            {
                _Processed = value;
                NotifyChanged( "Processed" );
            }
        }

        private bool _ProcessSuccess = false;
        public bool ProcessSuccess
        {
            get { return _ProcessSuccess; }
            private set
            {
                _ProcessSuccess = value;
                NotifyChanged( "ProcessSuccess" );
            }
        }

        private bool _Loading = false;
        public bool IsLoading
        {
            get { return _Loading; }
            set
            {
                _Loading = value;
                NotifyChanged( "IsLoading" );
            }
        }
        #endregion

        public GrantProcess( SHGrant GrantDef )
        {
            this.GrantDef = GrantDef;
            _Title = GrantDef.ScriptName;

            if ( GrantDef.SourceRemoved ) Processed = true;
        }

        public async void Parse( IEnumerable<CryptRSA> Crypts )
        {
            if ( Processed || NGrants == 0 ) return;
            IsLoading = true;

            switch ( GrantDef.Target )
            {
                case SHTarget.KEY:
                    // Download that script for decryption testing
                    string AccessToken = ( string ) new TokenManager().GetAuthById( GrantDef.ScriptId )?.Value;

                    TaskCompletionSource<string> ScriptData = new TaskCompletionSource<string>();
                    new RuntimeCache().POST(
                        Shared.ShRequest.Server
                        , Shared.ShRequest.ScriptDownload( GrantDef.ScriptId, AccessToken )
                        , ( e, id ) =>
                        {
                            try
                            {
                                ScriptData.SetResult( e.ResponseString );
                            }
                            catch ( Exception ex )
                            {
                                Logger.Log( ID, ex.Message, LogType.WARNING );
                                ScriptData.SetCanceled();
                            }
                        }
                        , ( a, b, c ) => { ScriptData.SetCanceled(); }
                        , true
                    );

                    string EncData;
                    try
                    {
                        JsonObject JDef = JsonStatus.Parse( await ScriptData.Task );
                        EncData = JDef.GetNamedString( "data" );
                    }
                    catch ( OperationCanceledException )
                    {
                        goto ProcessEnd;
                    }
                    catch( Exception ex )
                    {
                        FatalError( ex.Message );
                        return;
                    }

                    AESManager AesMgr = new AESManager();
                    foreach ( string EncCipher in GrantDef.Grants )
                    {
                        foreach ( CryptRSA Crypt in Crypts )
                        {
                            try
                            {
                                // Decrypt the key then use it to decrypt the Script data
                                string DecCipher = Crypt.Decrypt( EncCipher );
                                CryptAES AES = new CryptAES( DecCipher );
                                AES.Decrypt( EncData );

                                AesMgr.ImportAuth( GrantDef.ScriptName, DecCipher );
                                AesMgr.AssignId( GrantDef.ScriptName, GrantDef.ScriptId );

                                goto ProcessSuccess;
                            }
                            catch ( Exception ex )
                            {
                                Logger.Log( ID, ex.Message, LogType.WARNING );
                            }
                        }
                    }

                    goto ProcessEnd;

                case SHTarget.TOKEN:
                    TokenManager TokMgr = new TokenManager();
                    foreach ( string EncCipher in GrantDef.Grants )
                    {
                        foreach ( CryptRSA Crypt in Crypts )
                        {
                            try
                            {
                                string Token = Crypt.Decrypt( EncCipher );
                                TokMgr.ImportAuth( GrantDef.ScriptName, Token );
                                TokMgr.AssignId( GrantDef.ScriptName, GrantDef.ScriptId );

                                goto ProcessSuccess;
                            }
                            catch ( Exception ex )
                            {
                                Logger.Log( ID, "Failed to decyrpt: " + ex.Message, LogType.INFO );
                            }
                        }
                    }
                    goto ProcessEnd;

                default:
                    Logger.Log( ID, "Invalid Target: " + GrantDef.Target.ToString(), LogType.WARNING );
                    goto ProcessEnd;
            }

            ProcessSuccess:
            ProcessSuccess = true;

            // Close this request
            var j = Withdraw();

            ProcessEnd:
            IsLoading = false;
            Processed = true;

            if ( !ProcessSuccess ) ClearGrants();
        }

        public async Task<bool> Withdraw()
        {
            TaskCompletionSource<bool> TCS = new TaskCompletionSource<bool>();

            new RuntimeCache().POST(
                Shared.ShRequest.Server
                , Shared.ShRequest.WithdrawRequest( GrantDef.Id )
                , ( e, id ) =>
                {
                    try
                    {
                        JsonStatus.Parse( e.ResponseString );
                        TCS.TrySetResult( true );
                    }
                    catch ( Exception ex )
                    {
                        Logger.Log( ID, "Unable to withdraw request: " + ex.Message, LogType.WARNING );
                        TCS.TrySetResult( false );
                    }
                }
                , ( a, b, ex ) =>
                {
                    Logger.Log( ID, ex.Message, LogType.WARNING );
                    TCS.TrySetResult( false );
                }
                , false
            );

            return await TCS.Task;
        }

        // As all the grantings are not valid
        // We should remove all grant records at once
        private void ClearGrants()
        {
            new RuntimeCache().POST(
                Shared.ShRequest.Server
                , Shared.ShRequest.ClearGrants( GrantDef.Id )
                , Utils.DoNothing
                , Utils.DoNothing
                , false
            );

            Worker.UIInvoke( () =>
            {
                StringResources stx = new StringResources();
                Title = stx.Text( "ProcessGrantsFailed" );
            } );
        }

        private void FatalError( string Mesg )
        {
            IsLoading = false;
            Processed = true;

            Title = Mesg;
        }
    }
}