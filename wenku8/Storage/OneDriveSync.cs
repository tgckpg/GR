using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.OneDrive;
using Microsoft.OneDrive.Sdk;

using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;

namespace wenku8.Storage
{
    using Config;
    using Resources;
    using Settings;

    sealed class OneDriveSync
    {
        public static readonly string ID = typeof( OneDriveSync ).Name;

        public static OneDriveSync Instance;

        private IOneDriveClient Client;

        public bool Authenticated { get { return Client != null; } }

        public enum SyncMode
        {
            WITH_DEL_FLAG, AUTO
        }

        private readonly string[] scopes = new string[] { "onedrive.appfolder", "wl.signin" };

        public OneDriveSync()
        {

        }

        public async Task Authenticate()
        {
            if ( !Properties.ENABLE_ONEDRIVE ) return;

            if ( Authenticated ) return;
            OneDriveClient Client = OneDriveClientExtensions.GetUniversalClient( scopes ) as OneDriveClient;
            try
            {
                await Client.AuthenticateAsync();
                this.Client = Client;
                Logger.Log( ID, "Signed In", LogType.INFO );
            }
            catch ( OneDriveException ex )
            {
                Logger.Log( ID, ex.Error.Message, LogType.WARNING );
                Client.Dispose();
            }
        }

        public async Task UnAuthenticate()
        {
            if ( Client == null ) return;

            try
            {
                await Client.SignOutAsync();

                ( Client as OneDriveClient ).Dispose();
                Client = null;

                Logger.Log( ID, "Signed Out" );
            }
            catch( OneDriveException ex )
            {
                Logger.Log( ID, ex.Error.Message, LogType.WARNING );
            }
            catch ( SEHException ex )
            {
                Logger.Log( ID, ex.Message, LogType.ERROR );
            }
        }

        public async Task<bool> PushFile( string Location, Stream S )
        {
            if ( !Authenticated ) return false;

            try
            {
                await Client.Drive.Special.AppRoot
                    .ItemWithPath( Location )
                    .Content.Request()
                    .PutAsync<Item>( S );

                Logger.Log( ID, string.Format( "Pushed file {0}", Location ), LogType.DEBUG );
                return true;
            }
            catch ( OneDriveException ex )
            {
                Logger.Log( ID, ex.Error.Message, LogType.WARNING );
            }
            catch ( SEHException ex )
            {
                Logger.Log( ID, ex.Message, LogType.ERROR );
            }

            return false;
        }

        public async Task<Item> PullFile( string Location )
        {
            if ( !Authenticated ) return null;

            try
            {
                Item File = await Client.Drive.Special.AppRoot
                    .ItemWithPath( Location )
                    .Request()
                    .GetAsync();

                if( File != null )
                {
                    File.Content = await Client.Drive.Items[ File.Id ].Content.Request().GetAsync();
                }

                Logger.Log( ID, string.Format( "Pulled file {0}", Location ), LogType.DEBUG );
                return File;
            }
            catch( OneDriveException ex )
            {
                Logger.Log( ID, ex.Error.Message, LogType.WARNING );
            }
            catch ( SEHException ex )
            {
                Logger.Log( ID, ex.Message, LogType.ERROR );
            }

            return null;
        }

        public async Task SyncRegistry( XRegistry Reg, SyncMode Mode = SyncMode.WITH_DEL_FLAG )
        {
            // OneDrive Handler
            Item File = await PullFile( Reg.Location );
            if( File != null )
            {
                StreamReader SR = new StreamReader( File.Content );
                string Content = await SR.ReadToEndAsync();
                SR.Dispose();

                switch( Mode )
                {
                    case SyncMode.WITH_DEL_FLAG:
                        Reg.Merge(
                            new XRegistry( Content, null )
                            , ( XParameter LHS, XParameter RHS ) =>
                            {
                                return RHS.GetSaveLong( AppKeys.LBS_TIME ) <= LHS.GetSaveLong( AppKeys.LBS_TIME );
                            }
                        );
                        break;

                    case SyncMode.AUTO:
                        Reg.Sync(
                            new XRegistry( Content, null )
                            , Shared.Storage.FileExists( Reg.Location )
                              && File.LastModifiedDateTime < Shared.Storage.FileTime( Reg.Location )
                            , ( XParameter LHS, XParameter RHS ) =>
                            {
                                return RHS.GetSaveLong( AppKeys.LBS_TIME ) <= LHS.GetSaveLong( AppKeys.LBS_TIME );
                            }
                        );
                        break;

                }
                Reg.Save();
            }

            try
            {
                if ( Shared.Storage.FileExists( Reg.Location ) )
                {
                    Logger.Log( ID, "Pushing Storage Settings to OneDrive", LogType.INFO );
                    await PushFile(
                        Reg.Location
                        , Shared.Storage.GetStream( Reg.Location )
                    );
                }
            }
            catch( Exception ex )
            {
                Logger.Log( ID, "Failed to push Settings: " + ex.Message, LogType.ERROR );
            }
        }

    }
}