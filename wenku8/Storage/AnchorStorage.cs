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
	using Resources;
	using Model.Book;
	using Settings;

	class CustomAnchor : ActiveData
	{
		protected XRegistry Reg;
		protected string aid;

		public CustomAnchor( BookItem b )
		{
			Reg = new XRegistry( AppKeys.LBS_AXML, FileLinks.ROOT_ANCHORS + b.Id + ".xml" );
			Reg.SetParameter( AppKeys.GLOBAL_META, new XKey( AppKeys.GLOBAL_NAME, b.Title ) );
			aid = b.Id;
		}

		virtual public async Task SyncSettings()
		{
			await OneDriveSync.Instance.SyncRegistry( Reg );
		}

		public void SetCustomAnc( string cid, string name, int index, string color )
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

		public IEnumerable<XParameter> GetCustomAncs( string cid )
		{
			XParameter[] Params = Reg.Parameters();

			if ( Params == null ) return null;

			return Params.Where( ( XParameter P ) =>
			{
				return
					!P.GetBool( AppKeys.LBS_DEL, false )
					&& P.Id.IndexOf( cid + ":" ) == 0
				;
			} );
		}

		internal void RemoveCustomAnc( string cid, int anchorIndex )
		{
			XParameter P = Reg.Parameter( cid + ":" + anchorIndex );
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

	sealed class AutoAnchor : CustomAnchor
	{
		public static readonly string ID = typeof( AutoAnchor ).Name;

		private string VOL_ANC;
		private OAutoAnchor OAnchors;

		public AutoAnchor( BookItem b )
			: base( b )
		{
			VOL_ANC = ( b.IsDeathBook() ? "ALT.D." : "" ) + AppKeys.LBS_CH;
			OAnchors = new OAutoAnchor();
		}

		public void SaveAutoVolAnc( string cid )
		{
			try
			{
				Reg.SetParameter( VOL_ANC, new XKey[] { new XKey( AppKeys.GLOBAL_CID, cid ), BookStorage.TimeKey } );
			}
			catch ( Exception ex )
			{
				Logger.Log( ID, ex.Message, LogType.ERROR );
			}

			Reg.Save();
		}

		public void SaveAutoChAnc( string cid, int index )
		{
			string Id = "CAnc:" + cid;

			try
			{
				Reg.SetParameter( Id, new XKey[] { new XKey( AppKeys.LBS_INDEX, index.ToString() ), BookStorage.TimeKey } );
			}
			catch ( Exception ex )
			{
				Logger.Log( ID, ex.Message, LogType.ERROR );
			}

			Reg.Save();
		}

		public string GetAutoVolAnc()
		{
			XParameter XParam = Reg.Parameter( VOL_ANC );

			if( XParam == null )
			{
				string cid = OAnchors.GetBookmark( aid );
				if( !string.IsNullOrEmpty( cid ) )
				{
					SaveAutoVolAnc( cid );
					return cid;
				}
			}
			else
			{
				return XParam.GetValue( AppKeys.GLOBAL_CID );
			}

			return null;
		}

		public int GetAutoChAnc( string cid )
		{
			XParameter XParam = Reg.Parameter( "CAnc:" + cid );

			if( XParam == null )
			{
				int OReadAnchor = OAnchors.GetReadAnchor( cid );
				if( 0 < OReadAnchor )
				{
					SaveAutoChAnc( cid, OReadAnchor );
					return OReadAnchor;
				}
			}
			else
			{
				return XParam.GetSaveInt( AppKeys.LBS_INDEX );
			}

			return 0;
		}

		public override async Task SyncSettings()
		{
			await OAnchors.SyncSettings();
			await base.SyncSettings();
		}
	}

	[Obsolete( "ReadingAnchors.xml will be removed after Dec 2017" )]
	sealed class OAutoAnchor
	{
		private const string RFileName = FileLinks.ROOT_SETTING + FileLinks.READING_ANCHORS;

		private XRegistry WBookAnchors;

		public OAutoAnchor()
		{
			WBookAnchors = new XRegistry( AppKeys.LBS_AXML, RFileName );
		}

		public string GetBookmark( string aid )
		{
			XParameter p = WBookAnchors.Parameter( aid );
			if ( p != null )
				return p.GetValue( AppKeys.GLOBAL_CID );
			return null;
		}

		public int GetReadAnchor( string cid )
		{
			XParameter p = WBookAnchors.Parameter( cid );
			if( p!= null )
				return int.Parse( p.GetValue( AppKeys.LBS_INDEX ) );
			return 0;
		}

		public async Task SyncSettings()
		{
			if ( !Shared.Storage.FileExists( RFileName ) )
			{
				await OneDriveSync.Instance.SyncRegistry( WBookAnchors, OneDriveSync.SyncMode.FAVOR_REMOTE );
			}
		}

	}
}