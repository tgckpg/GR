using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;

namespace GR.Storage
{
	using Model.Book;
	using Settings;

	class CustomAnchor : ActiveData
	{
		protected XRegistry Reg;
		protected string OPath;

		public static XKey TimeKey
		{
			get { return new XKey( AppKeys.LBS_TIME, DateTime.Now.ToFileTimeUtc() ); }
		}

		public CustomAnchor( BookItem b )
		{
			if ( b.Type == Database.Models.BookType.S && b.ZoneId != AppKeys.ZLOCAL )
			{
				OPath = FileLinks.ROOT_ANCHORS + "Z" + b.PathId + ".xml";
			}
			else
			{
				OPath = FileLinks.ROOT_ANCHORS + b.ZItemId + ".xml";
			}

			string NPath = FileLinks.ROOT_ANCHORS + b.PathId + ".xml";
			Reg = new XRegistry( AppKeys.LBS_AXML, NPath );
			Reg.SetParameter( AppKeys.GLOBAL_META, new XKey( AppKeys.GLOBAL_NAME, b.Title ) );
		}

		virtual public async Task SyncSettings()
		{
			await OneDriveSync.Instance.SyncRegistry( Reg, OneDriveSync.SyncMode.WITH_DEL_FLAG, OPath );
			await OneDriveSync.Instance.RemoveFile( OPath );
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
				, TimeKey
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
				, TimeKey
			} );

			Reg.SetParameter( P );
			Reg.Save();
		}
	}

	sealed class AutoAnchor : CustomAnchor
	{
		public static readonly string ID = typeof( AutoAnchor ).Name;

		private string VOL_ANC;
		private ConcurrentDictionary<string, int> DelayedWrite;
		private volatile bool WritePending;

		public AutoAnchor( BookItem b )
			: base( b )
		{
			VOL_ANC = ( b.IsDeathblow() ? "ALT.D." : "" ) + AppKeys.LBS_CH;
			DelayedWrite = new ConcurrentDictionary<string, int>();
		}

		public void SaveAutoVolAnc( string cid )
		{
			try
			{
				Reg.SetParameter( VOL_ANC, new XKey[] { new XKey( AppKeys.GLOBAL_CID, cid ), TimeKey } );
			}
			catch ( Exception ex )
			{
				Logger.Log( ID, ex.Message, LogType.ERROR );
			}

			Reg.Save();
		}

		public async void SaveAutoChAnc( string cid, int index )
		{
			DelayedWrite[ cid ] = index;

			if ( WritePending )
				return;

			WritePending = true;
			await Task.Delay( 5000 );

			if ( !DelayedWrite.TryRemove( cid, out index ) )
			{
				return;
			}

			string Id = "CAnc:" + cid;

			try
			{
				Reg.SetParameter( Id, new XKey[] { new XKey( AppKeys.LBS_INDEX, index.ToString() ), TimeKey } );
			}
			catch ( Exception ex )
			{
				Logger.Log( ID, ex.Message, LogType.ERROR );
			}

			Reg.Save();
			WritePending = false;
		}

		public string GetAutoVolAnc()
		{
			XParameter XParam = Reg.Parameter( VOL_ANC );

			if( XParam != null )
			{
				return XParam.GetValue( AppKeys.GLOBAL_CID );
			}

			return null;
		}

		public int GetAutoChAnc( string cid )
		{
			XParameter XParam = Reg.Parameter( "CAnc:" + cid );

			if( XParam != null )
			{
				return XParam.GetSaveInt( AppKeys.LBS_INDEX );
			}

			return 0;
		}

		public override async Task SyncSettings()
		{
			await base.SyncSettings();
		}
	}
}