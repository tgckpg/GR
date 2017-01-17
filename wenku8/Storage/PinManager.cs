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

    sealed class PinManager
    {
        private const string LFileName = FileLinks.ROOT_SETTING + FileLinks.PIN_REGISTRY;

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

        public void RegPin( BookItem Book, bool Save = true )
        {
            LocalPins.SetParameter( Book.Id, new XKey[]
            {
                new XKey( AppKeys.GLOBAL_NAME, Book.Title )
                , BookStorage.TimeKey
            } );

            if ( Save )
            {
                PRegistry.SetParameter( LocalPins );
                PRegistry.Save();
            }
        }

        public IEnumerable<PinRecord> GetPinRecords()
        {
            IEnumerable<XParameter> DeviceParams = PRegistry.Parameters().Where( x => !x.GetBool( AppKeys.LBS_DEL ) );

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

    }
}