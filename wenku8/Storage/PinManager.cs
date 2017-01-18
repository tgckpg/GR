using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.IO;
using Net.Astropenguin.Linq;
using Net.Astropenguin.Logging;

namespace wenku8.Storage
{
    using Config;
    using Model.Book;
    using Model.ListItem;
    using Settings;

    enum PinPolicy {
        DO_NOTHING = 0, ASK = 1, PIN_MISSING = 2, REMOVE_MISSING = 3
    }

    sealed class PinManager
    {
        private const string LFileName = FileLinks.ROOT_SETTING + FileLinks.PIN_REGISTRY;

        public PinPolicy Policy
        {
            get
            {
                return ( PinPolicy ) LocalPins.GetSaveInt( AppKeys.PM_POLICY );
            }
            set
            {
                LocalPins.SetValue( new XKey[] {
                    new XKey( AppKeys.PM_POLICY, ( int ) value )
                    , BookStorage.TimeKey
                } );
                Save();
            }
        }

        private XRegistry PRegistry;
        private XParameter LocalPins;

        public PinManager()
        {
            PRegistry = new XRegistry( AppKeys.LBS_BXML, LFileName );

            LocalPins = PRegistry.Parameter( AppSettings.DeviceId );
            if ( LocalPins == null )
            {
                LocalPins = new XParameter( AppSettings.DeviceId );
                LocalPins.SetValue( new XKey[] {
                    new XKey( AppKeys.GLOBAL_NAME, AppSettings.DeviceName )
                    , BookStorage.TimeKey
                } );
            }
        }

        public async Task SyncSettings()
        {
            await OneDriveSync.Instance.SyncRegistry( PRegistry );
        }

        public void RegPin( BookItem Book, string TileId, bool Save = true )
        {
            LocalPins.SetParameter( Book.Id, new XKey[]
            {
                new XKey( AppKeys.GLOBAL_NAME, Book.Title )
                , new XKey( AppKeys.GLOBAL_RID, TileId )
                , BookStorage.TimeKey
            } );

            ActivateLocalPins();

            if ( Save )
            {
                PRegistry.SetParameter( LocalPins );
                PRegistry.Save();
            }
        }

        public void RemoveDev( string DeviceId )
        {
            PRegistry.RemoveParameter( DeviceId );

            PRegistry.SetParameter( DeviceId, new XKey[]
            {
                new XKey( AppKeys.LBS_DEL, true )
                , BookStorage.TimeKey
            } );

            LocalPins = PRegistry.Parameter( DeviceId );

            Save();
        }

        public void RemovePin( string Id )
        {
            LocalPins.RemoveParameter( Id );
            ActivateLocalPins();
            Save();
        }

        public void RemovePin( IEnumerable<string> Ids )
        {
            foreach ( string Id in Ids )
                LocalPins.RemoveParameter( Id );

            ActivateLocalPins();
            Save();
        }

        public IEnumerable<ActiveItem> GetLocalPins()
        {
            return LocalPins.GetParameters().Remap( x => new ActiveItem(
                x.GetValue( AppKeys.GLOBAL_NAME )
                , x.Id
                , x.GetValue( AppKeys.GLOBAL_RID )
            ) ).ToArray();
        }

        public IEnumerable<PinRecord> GetPinRecords()
        {
            IEnumerable<XParameter> DeviceParams = PRegistry.Parameters()
                .Where( x => !x.GetBool( AppKeys.LBS_DEL ) )
                .OrderByDescending( x => x.Id == AppSettings.DeviceId );

            List<PinRecord> PinRecords = new List<PinRecord>();
            foreach ( XParameter DeviceParam in DeviceParams )
            {
                PinRecord DeviceRecord = new PinRecord( DeviceParam.GetValue( AppKeys.GLOBAL_NAME ), DeviceParam.Id );
                PinRecords.Add( DeviceRecord );

                XParameter[] PinParams = DeviceParam.GetParameters();

                foreach ( XParameter PinParam in PinParams )
                {
                    PinRecords.Add( new PinRecord(
                        PinParam.Id
                        , DeviceParam.Id
                        , PinParam.GetValue( AppKeys.GLOBAL_NAME )
                        , PinParam.GetValue( AppKeys.GLOBAL_RID )
                        , DateTime.FromFileTimeUtc( PinParam.GetSaveLong( AppKeys.LBS_TIME ) )
                    ) );
                }
            }

            return PinRecords;
        }

        public void Save()
        {
            PRegistry.SetParameter( LocalPins );
            PRegistry.Save();
        }

        private void ActivateLocalPins()
        {
            LocalPins.SetValue( new XKey[] {
                new XKey( AppKeys.LBS_DEL, false )
                , new XKey( AppKeys.GLOBAL_NAME, AppSettings.DeviceName )
                , BookStorage.TimeKey
            } );
        }
    }
}