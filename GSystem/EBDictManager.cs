using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.IO;
using Net.Astropenguin.Linq;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;

using libeburc;

namespace GR.GSystem
{
	using Model;
	using Model.ListItem;
	using Resources;
	using Settings;

	sealed class EBDictManager : ActiveData
	{
		public static readonly string ID = typeof( EBDictManager ).Name;

		#region Display Properties
		private bool _isloading = false;
		public bool Loading
		{
			get { return _isloading; }
			set { _isloading = value; NotifyChanged( "Loading" ); }
		}

		private bool _inst = false;
		public bool Installing
		{
			get { return _inst; }
			set { _inst = value; NotifyChanged( "Installing" ); }
		}

		private string _instmesg = "";
		public string InstMessage
		{
			get { return _instmesg; }
			set { _instmesg = value; NotifyChanged( "InstMessage" ); }
		}

		private bool _havechoice = false;
		public bool HaveChosen
		{
			get { return _havechoice; }
			set { _havechoice = value;  NotifyChanged( "HaveChosen" ); }
		}

		public async void Remove( ActiveItem activeItem )
		{
			await Task.Run(
				() => {
					string id = activeItem.Payload;
					Shared.Storage.RemoveDir( FileLinks.ROOT_EBWIN + id );
					DictReg.RemoveParameter( id );
					DictReg.Save();
				}
			);
			__CachedDicts = null;
			UpdateDictList();
		}

		public static void SetLogger()
		{
			EBLogger.RegisterHandler( EBLog );
		}

		private static void EBLog( string Message )
		{
			Logger.Log( "libeburc", Message, LogType.INFO );
		}

		private bool _canInstall;
		public bool CanInstall
		{
			get { return _canInstall; }
			set { _canInstall = value;  NotifyChanged( "CanInstall" ); }
		}

		private IEnumerable<ActiveItem> _data;
		public IEnumerable<ActiveItem> Data
		{
			get { return _data; }
			set { _data = value;  NotifyChanged( "Data" ); }
		}
		#endregion

		private IStorageFolder SrcFolder;
		private XParameter SrcDict;

		private XRegistry DictReg;

		private static EBDictionary __CachedDicts;

		public EBDictManager()
			:base()
		{
			DictReg = new XRegistry( "<Dicts />", FileLinks.ROOT_SETTING + FileLinks.EBWIN_DICT_REG );
			UpdateDictList();
		}

		public async void OpenNewDict()
		{
			if ( Installing ) return;

			SrcDict = new XParameter( "Dict" );
			CanInstall = false;
			Installing = true;
			try
			{
				IStorageFolder IFolder = await AppStorage.OpenDirAsync();
				if ( !( HaveChosen = ( IFolder != null ) ) )
				{
					goto End;
				}

				SrcFolder = IFolder;

				Loading = true;
				EBBook Book = await EBBook.Parse( IFolder );

				StringResources stx = StringResources.Load( "Settings" );
				string size = Utils.AutoByteUnit( await Shared.Storage.CountSizeRecursive( SrcFolder ) );
				string fmt = "{0}: {1}";
				string Message = string.Format( fmt, stx.Text( "EBWin_DiscType" ), EBInfo.DiskType( Book ) )
					+ "\n" + string.Format( fmt, stx.Text( "EBWin_CharCode" ), EBInfo.CharCode( Book ) )
					+ "\n" + string.Format( fmt, stx.Text( "EBWin_NumSubbooks" ), Book.SubbookCount )
					+ "\n" + string.Format( fmt, stx.Text( "Size" ), size )
					;

				bool TitleSet = false;
				foreach ( EBSubbook Subbook in Book.Subbooks )
				{
					await Subbook.OpenAsync();
					Message += "\n" + Subbook.Title;
					if ( !TitleSet )
					{
						TitleSet = true;
						SrcDict.SetValue( new XKey( "Title", Subbook.Title ) );
						SrcDict.SetValue( new XKey( "Size", size ) );
					}
				}

				IStorageFile CATALOG;
				try
				{
					CATALOG = await SrcFolder.GetFileAsync( "CATALOGS" );
				}
				catch( FileNotFoundException )
				{
					CATALOG = await SrcFolder.GetFileAsync( "CATALOG" );
				}

				string hash = await Utils.Sha1( CATALOG );
				SrcDict.Id = hash;

				CanInstall = !Data.Any( x => x.Payload == hash );

				if( !CanInstall )
				{
					Message += "\n" + stx.Text( "EBWin_Already_Installed" );
				}

				InstMessage = Message;
			}
			catch ( Exception ex )
			{
				EBErrorCode Code = EBException.LastError;
				if ( Code != EBErrorCode.EB_SUCCESS )
				{
					InstMessage = Code.ToString();
				}
				else
				{
					InstMessage = ex.Message;
				}
			}

			End: // End
			Loading = false;
			Installing = false;
		}

		public async void Install()
		{
			if ( Installing || !CanInstall ) return;
			Installing = true;
			Loading = true;

			StringResources stx = StringResources.Load( "Settings" );

			try
			{
				IStorageFolder TargetFolder = await Shared.Storage.CreateDirFromISOStorage(
					FileLinks.ROOT_EBWIN + SrcDict.Id 
				);

				SrcDict.SetValue( new XKey( "Corrupted", true ) );
				DictReg.SetParameter( SrcDict );
				DictReg.Save();

				Func<IStorageFolder, IStorageFolder, Task> Copie = null;
				Copie = async ( IStorageFolder SrcDir, IStorageFolder TargetDir ) =>
				{
					IEnumerable<IStorageFile> files = await SrcDir.GetFilesAsync();
					foreach ( IStorageFile f in files )
					{
						InstMessage = stx.Text( "EBWin_Installing" ) + ": " + f.Name;
						await f.CopyAsync( TargetDir, f.Name, NameCollisionOption.ReplaceExisting );
					}
					IEnumerable<IStorageFolder> dirs = await SrcDir.GetFoldersAsync();
					foreach ( IStorageFolder dir in dirs )
					{
						InstMessage = stx.Text( "EBWin_Installing" ) + ": " + dir.Name;
						IStorageFolder f = await TargetDir.CreateFolderAsync( dir.Name, CreationCollisionOption.OpenIfExists );
						await Copie( dir, f );
					}
				};
				await Copie( SrcFolder, TargetFolder );

				SrcDict.SetValue( new XKey( "Corrupted", false ) );
				DictReg.SetParameter( SrcDict );
				DictReg.Save();

				__CachedDicts = null;
				InstMessage = "";
			}
			catch ( Exception ex )
			{
				InstMessage = stx.Text( "EBWin_Install_Failed" ) + ": " + ex.Message;
			}

			UpdateDictList();
			CanInstall = false;
			Loading = false;
			Installing = false;
		}

		public async Task<EBDictionary> GetDictionary( string id = null )
		{
			if ( __CachedDicts != null ) return __CachedDicts;

			List<EBSubbook> Subbooks = new List<EBSubbook>();
			Func<string, bool> Equal;

			if ( id == null ) Equal = x => true;
			else Equal = x => x == id;

			InstMessage = "";

			foreach ( ActiveItem Dict in Data )
			{
				if ( !Equal( Dict.Payload ) ) continue;

				string bid = Dict.Payload;
				IStorageFolder dir = await Shared.Storage.CreateDirFromISOStorage( FileLinks.ROOT_EBWIN + bid );
				try
				{
					EBBook book = await EBBook.Parse( dir );
					foreach ( EBSubbook subbook in book.Subbooks )
					{
						await subbook.OpenAsync();
						Subbooks.Add( subbook );
					}
				}
				catch ( Exception ex )
				{
					EBErrorCode Code = EBException.LastError;
					if ( Code != EBErrorCode.EB_SUCCESS )
					{
						InstMessage = Code.ToString();
					}
					else
					{
						InstMessage = ex.Message;
					}

					XParameter XParam = DictReg.Parameter( Dict.Payload );
					XParam.SetValue( new XKey( "Corrupted", true ) );
					DictReg.SetParameter( XParam );
				}
			}

			if ( !string.IsNullOrEmpty( InstMessage ) )
			{
				DictReg.Save();
			}

			return ( __CachedDicts = new EBDictionary( Subbooks ) );
		}

		private void UpdateDictList()
		{
			IEnumerable<XParameter> Params = DictReg
				.Parameters( "Title" )
				.Where( x => !x.GetBool( "Corrupted" ) );

			StringResources stx = StringResources.Load( "Settings" );
			Data = Params.Select( x => new ActiveItem( x.GetValue( "Title" ), x.GetValue( "Size" ), x.Id ) );
		}
	}
}