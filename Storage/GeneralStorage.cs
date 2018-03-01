using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Windows.Storage;

using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;
using Net.Astropenguin.Helpers;

namespace GR.Storage
{
	using Settings;

	sealed class GeneralStorage : AppStorage
	{
		new public readonly string ID = typeof( GeneralStorage ).Name;

		public bool IsLibraryValid = false;

		public static HashSet<string> CachedFiles = new HashSet<string>();

		public GeneralStorage()
		{
			var j = InitializeAsync();
		}

		public async Task InitializeAsync()
		{
			string[] RootList = {
				// BackgroundTransfer and ShellTile Roots
				FileLinks.ROOT_SHARED
				, FileLinks.ROOT_BACKGROUNDSERVICE
				// Cover Cache
				, FileLinks.ROOT_COVER
				// Banner Cache
				, FileLinks.ROOT_BANNER
				// Tiles
				, FileLinks.ROOT_TILE
				// Illustration Cache
				, FileLinks.ROOT_IMAGE
				// Spider Volumes
				, FileLinks.ROOT_SPIDER_VOL
				// Comments, SpecialTopics, etc.
				, FileLinks.ROOT_WTEXT
				// Settings, LocalBookStorage and some future staffs.
				, FileLinks.ROOT_SETTING
				// Anchors, Every custom book anchors
				, FileLinks.ROOT_ANCHORS
				// EBWin, dictionaries
				, FileLinks.ROOT_EBWIN
				// ZoneSpider Definitions
				, FileLinks.ROOT_ZSPIDER
				// ContentReader Thumbnails
				, FileLinks.ROOT_READER_THUMBS
				// Log files
				, FileLinks.ROOT_LOG
			};

			foreach ( string i in RootList )
			{
				if ( !DirExist( i ) )
					CreateDirectory( i );
			}

			await InitAppLibrary( "wenku8" );
			await TestLibraryValid();
			await ClearTemp();
		}

		public async Task GetLocalText( Func<StorageFile, int, int, Task> Initializer )
		{
			try
			{
				IReadOnlyList<StorageFile> Files = await PickFolderForFiles();

				if ( Files == null ) return;

				int l = Files.Count;
				int i = 0;
				foreach ( StorageFile F in Files )
				{
					if ( F.ContentType == "text/plain" )
					{
						await Initializer( F, i++, l );
					}
					else
					{
						l--;
					}
				}
			}
			catch ( Exception ) { }
		}

		public string PicId( string name )
		{
			if( name.Contains( "/" ) )
			{
				name = name.Substring( name.LastIndexOf( "/" ) + 1 );
			}
			return "wenku8_" + name;
		}

		new public async Task<bool> TestLibraryValid()
		{
			return IsLibraryValid = await base.TestLibraryValid();
		}

		new public async Task<bool> SearchLibrary( string id )
		{
			return IsLibraryValid && await SearchLibrary( PicId( id ) ); 
		}

		public async Task<AsyncTryOut<Stream>> TryGetImage( string id )
		{
			AsyncTryOut<Stream> ato = null;

			// Search Phone library
			if ( ato = await TryGetImgFromLib( PicId( id ) ) )
			{
				if ( FileExists( FileLinks.ROOT_IMAGE + id ) )
				{
					// Always try to remove the file locally if library have one
					try
					{
						Logger.Log( ID, "Image \"" + id + "\" exists in library, removing from app storage", LogType.INFO );
						DeleteFile( FileLinks.ROOT_IMAGE + id );
					}
					catch ( Exception ) { Logger.Log( ID, "Cannot remove image: " + id, LogType.INFO ); }
				}
				return ato;
			}

			// Search internal cache first
			if ( FileExists( FileLinks.ROOT_IMAGE + id ) )
			{
				Stream s = GetStream( FileLinks.ROOT_IMAGE + id );
				return new AsyncTryOut<Stream>( true, s );
			}
			return new AsyncTryOut<Stream>();
		}

		#region DELETE CONTENTS
		new public void PurgeContents( string Dir, bool RmRoot )
		{
			CachedFiles.Clear();
			base.PurgeContents( Dir, RmRoot );
		}

		public void CLEAR_COVER()
		{
			PurgeContents( FileLinks.ROOT_COVER, false );
		}

		public void CLEAR_IMAGE()
		{
			PurgeContents( FileLinks.ROOT_IMAGE, false );
		}
		#endregion

		override public bool WriteString( string filename, string content )
		{
			Logger.Log( ID, string.Format( "WritingString: {0}", filename ), LogType.DEBUG );
			CreateDirs( filename.Substring( 0, filename.LastIndexOf( '/' ) ) );
			return base.WriteString( filename, content );
		}

		override public bool WriteBytes( string filename, Byte[] b )
		{
			Logger.Log( ID, string.Format( "WriteBytes: {0}", filename ), LogType.DEBUG );
			CreateDirs( filename.Substring( 0, filename.LastIndexOf( '/' ) ) );
			return base.WriteBytes( filename, b );
		}

		override public bool WriteStream( string filename, Stream S )
		{
			Logger.Log( ID, string.Format( "WriteStream: {0}", filename ), LogType.DEBUG );
			CreateDirs( filename.Substring( 0, filename.LastIndexOf( '/' ) ) );
			return base.WriteStream( filename, S );
		}

		public void MoveDir( string From, string To )
		{
			Logger.Log( ID, string.Format( "Move: {0} -> {1}", From, To ), LogType.DEBUG );
			UserStorage.MoveDirectory( From, To );
			CachedFiles.RemoveWhere( x => x.StartsWith( From ) );
		}

		public void MoveFile( string From, string To )
		{
			Logger.Log( ID, string.Format( "Move: {0} -> {1}", From, To ), LogType.DEBUG );
			CreateDirs( To.Substring( 0, To.LastIndexOf( '/' ) ) );
			UserStorage.MoveFile( From, To );

			CachedFiles.Remove( From );
			CachedFiles.Add( To );
		}

		public async Task<bool> WriteFileAsync( string filename, IStorageFile ISF )
		{
			return WriteStream( filename, await ISF.OpenStreamForReadAsync() );
		}

		public void RemoveDir( string location )
		{
			if ( location[ location.Length - 1 ] != '/' ) location += '/';

			PurgeContents( location, true );
		}

		internal async Task<IStorageFile> GetImage( string saveLocation )
		{
			if ( Config.Properties.DATA_IMAGE_SAVE_TO_MEDIA_LIBRARY )
			{
				saveLocation = PicId( saveLocation );
				return await CreateImageFromLibrary( saveLocation );
			}

			CreateDirs( saveLocation.Substring( 0, saveLocation.LastIndexOf( '/' ) ) );
			return await CreateFileFromISOStorage( saveLocation );
		}

		public async Task<bool> SavePicture( string id, Stream s )
		{
			string filename = PicId( id );

			StorageFile f = await PicLibrary.CreateFileAsync( filename );
			Stream fs = await f.OpenStreamForWriteAsync();
			await s.CopyToAsync( fs );
			s.Dispose();
			fs.Dispose();

			f = await PicLibrary.TryGetItemAsync( filename ) as StorageFile;
			return f != null;
		}

		public async Task<bool> DeletePicture( string id )
		{
			string filename = PicId( id );
			StorageFile f = await PicLibrary.TryGetItemAsync( filename ) as StorageFile;
			if( f != null )
			{
				await f.DeleteAsync( StorageDeleteOption.Default );
			}

			f = await PicLibrary.TryGetItemAsync( filename ) as StorageFile;

			return f == null;
		}

		new public bool FileExists( string FileName )
		{
			if ( string.IsNullOrEmpty( FileName ) ) return false;
			if ( CFExists( FileName ) ) return true;

			if( base.FileExists( FileName ) )
			{
				CachedFiles.Add( FileName );
				return true;
			}

			return false;
		}

		new public bool DeleteFile( string FileName )
		{
			CachedFiles.Remove( FileName );
			return base.DeleteFile( FileName );
		}

		public bool FileChanged( string StringToCompare, string FileToCompare )
		{
			if( FileExists( FileToCompare ) )
			{
				return ( StringToCompare != GetString( FileToCompare ) );
			}

			return true;
		}

		public bool RemoveImage( string id )
		{
			return DeleteFile( FileLinks.ROOT_IMAGE + id );
		}

		#region Get Sizes
		public ulong CoverSize()
		{
			ulong size = 0;
			CountSize( FileLinks.ROOT_COVER, ref size );
			return size;
		}

		public ulong ImageSize()
		{
			ulong size = 0;
			CountSize( FileLinks.ROOT_IMAGE, ref size );
			return size;
		}

		public ulong GetTotalUsage()
		{
			ulong size = 0;
			CountSizeRecursive( "./", ref size );
			return size;
		}
		#endregion

		public void CacheFileStatus()
		{
			var j = Task.Run( () =>
			{
				CachedFiles.Clear();
				Find( "./", CachedFiles );
			} );
		}

		private bool CFExists( string FileName )
		{
			return CachedFiles.Contains( FileName );
		}

		private void Find( string Dir, HashSet<string> Files )
		{
			foreach ( string file in UserStorage.GetFileNames( Dir ) )
			{
				Files.Add( ( Dir + file ).Substring( 2 ) );
			}

			foreach ( string d in UserStorage.GetDirectoryNames( Dir ) )
			{
				Find( Dir + d + "/", Files );
			}
		}
	}
}