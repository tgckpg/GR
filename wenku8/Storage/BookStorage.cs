using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Net.Astropenguin.IO;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;

namespace wenku8.Storage
{
    using Settings;
    using Model.ListItem;

    sealed class BookStorage
	{
        public static readonly string ID = typeof( BookStorage ).Name;

		private const string LFileName = FileLinks.ROOT_SETTING + FileLinks.LOCAL_BOOK_STORAGE;

        public static XKey TimeKey
        {
            get
            {
                return new XKey( AppKeys.LBS_TIME, DateTime.Now.ToFileTimeUtc() );
            }
        }

		private XRegistry WBookStorage;


		public BookStorage()
		{
			WBookStorage = new XRegistry( AppKeys.LBS_BXML, LFileName );
		}

        public async Task SyncSettings()
        {
            await OneDriveSync.Instance.SyncRegistry( WBookStorage );
        }

        internal T[] GetList<T>()
        {
            Type t = typeof( T );
            if ( !( t == typeof( FavItem ) || t.GetTypeInfo().IsSubclassOf( typeof( FavItem ) ) ) )
            {
                throw new InvalidCastException( "Cannot cast " + t.Name + " into FavItem" );
            }

            IEnumerable<XParameter> p = WBookStorage
                .Parameters()
                .Where( x => !x.GetBool( AppKeys.LBS_DEL ) )
                .OrderBy( x => x.GetBool( AppKeys.LBS_NEW ) )
                .ToArray();

            StringResources Res = new StringResources( "Book" );

            List<T> s = new List<T>();
            foreach ( XParameter wp in p )
            {
                T Item = ( T ) Activator.CreateInstance(
                    typeof( T )
                    , wp.GetValue( AppKeys.GLOBAL_NAME )
                    , wp.GetValue( AppKeys.LBS_DATE )
                    , wp.GetValue( AppKeys.LBS_CH )
                    , wp.GetValue( AppKeys.GLOBAL_ID )
                    , wp.GetValue( AppKeys.LBS_WSYNC ) != null
                    , wp.GetValue( AppKeys.LBS_AUM ) != null
                    , wp.GetBool( AppKeys.LBS_NEW )
                );

                s.Add( Item );
            }

            s.Reverse();

            return s.ToArray();
        }


		public string[] GetIdList()
		{
			IEnumerable<XParameter> Params = WBookStorage.Parameters().Where( x => !x.GetBool( AppKeys.LBS_DEL, false ) );

			int i = 1;
            int l = Params.Count();

            string[] s = new string[ l ];

            foreach ( XParameter p in Params )
            {
                s[ ( l - i++ ) ] = p.GetValue( AppKeys.GLOBAL_ID );
            }

			return s;
		}

		public void RemoveBook( string id, bool Save = true )
		{
            XParameter P = WBookStorage.Parameter( id );
            if ( P == null ) return;
            P.SetValue( new XKey( AppKeys.LBS_DEL, true ) );

            WBookStorage.SetParameter( P );
			if( Save ) WBookStorage.Save();
		}

		public XParameter GetBook( string id )
		{
			XParameter P =  WBookStorage.Parameter( id );
            if ( P == null || P.GetBool( AppKeys.LBS_DEL ) ) return null;

            return P;
		}

		public void SaveBook( string id, string name, string date, string LastChapter, bool Save = true )
		{
			// If book was already was found
			XParameter p = GetBook( id );
			if ( p != null )
			{
				// Perform update
				if ( p.GetValue( AppKeys.LBS_DATE ) != date )
				{
					try
					{
                        p.SetValue(
                            new XKey( AppKeys.GLOBAL_NAME, name )
                            , new XKey( AppKeys.LBS_DATE, date )
                            , new XKey( AppKeys.LBS_CH, LastChapter )
                            , new XKey( AppKeys.LBS_NEW, true )
                            , new XKey( AppKeys.LBS_DEL, false )
                            , TimeKey
                        );

                        WBookStorage.SetParameter( p );
					}
					catch ( Exception ex )
					{
                        Logger.Log( ID, ex.Message, LogType.ERROR );
                    }
                }
			}
			else
			{
				WBookStorage.SetParameter( id, new XKey[] {
					new XKey( AppKeys.GLOBAL_ID, id )
					, new XKey( AppKeys.LBS_DATE, date )
					, new XKey( AppKeys.GLOBAL_NAME, name )
					, new XKey( AppKeys.LBS_CH, LastChapter )
					, new XKey( AppKeys.LBS_NEW, false )
                    , new XKey( AppKeys.LBS_DEL, false )
                    , TimeKey
				} );
			}

			if( Save ) WBookStorage.Save();
		}

		public bool BookExist( string id )
		{
            XParameter Param = WBookStorage.Parameter( id );
            if ( Param == null ) return false;

            return !Param.GetBool( AppKeys.LBS_DEL, false );
		}

		public void BookRead( string id )
		{
			SetBool( id, AppKeys.LBS_NEW, false );
            SaveBookStorage();
		}

		public bool AutoUpdateSwitch( string id )
		{
			XParameter p = WBookStorage.Parameter( id );
			if ( p.GetValue( AppKeys.LBS_AUM ) == null )
			{
				SetBool( id, AppKeys.LBS_AUM, true );
				return true;
			}

			RemoveKey( id, AppKeys.LBS_AUM );
			return false;
		}

		public bool SyncSwitch( string id, bool? status = null )
		{
			XParameter p = WBookStorage.Parameter( id );

            if( status != null )
            {
                bool S = ( bool ) status;
				SetBool( id, AppKeys.LBS_WSYNC, S );

                return S;
            }

			if ( p.GetValue( AppKeys.LBS_WSYNC ) == null )
			{
				SetBool( id, AppKeys.LBS_WSYNC, true );
				return true;
			}

			RemoveKey( id, AppKeys.LBS_WSYNC );
			return false;
		}

		public string[] GetSyncedList()
		{
			int l;
			XParameter[] p = WBookStorage.Parameters( AppKeys.LBS_WSYNC );
			string[] list = new string[l = p.Count() ];
			for ( int i = 0; i < l; i++ )
			{
				list[i] = p[i].Id;
			}
			return list;
		}

		private void SetBool( string id, string key, bool value )
		{
			XParameter p = WBookStorage.Parameter( id );
			if ( p != null )
			{
				p.SetValue( new XKey( key, value ) );
                p.SetValue( TimeKey );
                WBookStorage.SetParameter( p );
			}
		}

		private void RemoveKey( string id, string key )
		{
			XParameter p = WBookStorage.Parameter( id );
			if ( p != null )
			{
				p.RemoveKey( key );
                p.SetValue( TimeKey );
                WBookStorage.SetParameter( p );
			}
		}

		public string[] GetAutomations()
		{
			int l;
			XParameter[] p = WBookStorage.Parameters( AppKeys.LBS_AUM );
			string[] id = new string[l = p.Count() ];
			for ( int i = 0; i < l; i++ )
			{
				id[i] = p[i].Id;
			}
			return id;
		}

        public void SaveBookStorage()
        {
            WBookStorage.Save();
        }
	}
}