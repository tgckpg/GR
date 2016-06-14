using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;

namespace wenku8.Storage
{
    using Model;
    using Model.Book;
    using Settings;

    class CustomAnchor : ActiveData
    {
        XRegistry Reg;

        public CustomAnchor( BookItem b )
        {
            Reg = new XRegistry( AppKeys.LBS_AXML, FileLinks.ROOT_ANCHORS + b.Id + ".xml" );
        }

        public async Task SyncSettings()
        {
            await OneDriveSync.Instance.SyncRegistry( Reg );
        }

        public void SetAnchor( string cid, string name, int index, string color )
        {
            string Key = cid + ":" + index;
            Reg.SetParameter( Key, new XKey[] {
                new XKey( AppKeys.GLOBAL_CID, cid )
                , new XKey( AppKeys.LBS_ANCHOR, name )
                , new XKey( AppKeys.LBS_INDEX, index.ToString() )
                , new XKey( AppKeys.LBS_COLOR, color )
                , new XKey( AppKeys.LBS_DEL, false )
                , BookStorage.TimeKey
            } );
            Reg.Save();
        }

        public IEnumerable<XParameter> GetAnchors( string cid )
        {
            XParameter[] Params = Reg.GetParameters();

            if ( Params == null ) return null;

            return Params.Where( ( XParameter P ) =>
            {
                return
                    !P.GetBool( AppKeys.LBS_DEL, false )
                    && P.GetValue( AppKeys.GLOBAL_CID ) == cid
                ;
            } );
        }

        internal void RemoveAnchor( string cid, int anchorIndex )
        {
            XParameter P = Reg.GetParameter( cid + ":" + anchorIndex );
            if ( P == null ) return;
            P.ClearKeys();
            P.SetValue( new XKey[] {
                new XKey( AppKeys.LBS_DEL, true )
                , BookStorage.TimeKey
            } );

            Reg.SetParameter( P );
            Reg.Save();
        }
    }

    class AutoAnchor
    {
        public static readonly string ID = typeof( AutoAnchor ).Name;

		private const string RFileName = FileLinks.ROOT_SETTING + FileLinks.READING_ANCHORS;

		private XRegistry WBookAnchors;

        public AutoAnchor()
        {
			WBookAnchors = new XRegistry( AppKeys.LBS_AXML, RFileName );
        }

		public void SaveBookmark( string aid, string cid )
		{
			XParameter p = WBookAnchors.GetParameter( aid );
			if ( p != null )
			{
				// Perform update
				try
				{
					p.SetValue( new XKey( AppKeys.GLOBAL_CID, cid ) );
                    WBookAnchors.SetParameter( p );
				}
				catch ( Exception ex )
				{
                    Logger.Log( ID, ex.Message, LogType.ERROR );
				}
			}
			else
			{
				WBookAnchors.SetParameter( aid, new XKey( AppKeys.GLOBAL_CID, cid ) );
			}
			WBookAnchors.Save();
		}

		public string GetBookmark( string aid )
		{
			XParameter p = WBookAnchors.GetParameter( aid );
			if ( p != null )
				return p.GetValue( AppKeys.GLOBAL_CID );
			return null;
		}

		public void SaveAnchor( string cid, int index )
		{
			// If anchor is set
			XParameter p = WBookAnchors.GetParameter( cid );
			if ( p != null )
			{
				// Perform update
				try
				{
					p.SetValue(
                        new XKey( AppKeys.LBS_INDEX, index.ToString() )
                        , BookStorage.TimeKey
                    );
                    WBookAnchors.SetParameter( p );
				}
				catch ( Exception ex )
				{
                    Logger.Log( ID, ex.Message, LogType.ERROR );
				}
			}
			else
			{
				WBookAnchors.SetParameter( cid, new XKey[] {
					new XKey( AppKeys.GLOBAL_CID, cid )
					, new XKey( AppKeys.LBS_INDEX, index.ToString() )
                    , BookStorage.TimeKey
				} );
			}

			WBookAnchors.Save();
		}

		public int GetReadAnchor( string cid )
		{
			XParameter p = WBookAnchors.GetParameter( cid );
			if( p!= null )
				return int.Parse( p.GetValue( AppKeys.LBS_INDEX ) );
			return 0;
		}

        public async Task SyncSettings()
        {
            await OneDriveSync.Instance.SyncRegistry( WBookAnchors );
        }
    }
}
