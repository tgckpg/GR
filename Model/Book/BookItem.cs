using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;

namespace GR.Model.Book
{
	using Config;
	using Database.Models;
	using ListItem;
	using Net.Astropenguin.Linq;
	using Resources;
	using Settings;
	using System.Reflection;

	enum PropType
	{
		Title,
		DailyHitCount, TotalHitCount,
		FavCount, PushCount,
		PostingDate, LastUpdateDate,
		Author, Press, Length,
		Status, LatestSection,
		Cover, Intro, Others,
	}

	class BookItem : ActiveData
	{
		public static readonly string ID = typeof( BookItem ).Name;

		private static StringResources _Res;
		protected static StringResources Res => _Res ?? ( _Res = StringResources.Load( "Book" ) );

		public static string PropertyName( PropType InfoType ) => Res.Text( InfoType.ToString() );
		public static string PropertyName( PropertyInfo Property )
		{
			string LStr = Res.Text( Property.Name );

			if ( string.IsNullOrEmpty( LStr ) )
				return Property.Name;

			return LStr;
		}

		private string DisplayString( string Raw, PropType InfType, string Suffix = "" )
		{
			return string.IsNullOrEmpty( Raw ) ? "" : ( PropertyName( InfType ) + ": " + Raw + Suffix );
		}

		protected Book _Entry;
		public Book Entry => _Entry ?? throw new InvalidOperationException( "_Entry is not initialized" );

		public string GID => string.Format( "{0}.{1}.{2}", Entry.ZoneId, Entry.Type, Entry.ZItemId );
		public string PathId => ZoneId + "/" + ZItemId;

		public int Index { get; protected set; }

		public int Id
		{
			get { return Entry.Id; }
			set { Entry.Id = value; }
		}

		public string ZoneId
		{
			get { return Entry.ZoneId; }
			set { Entry.ZoneId = value; }
		}

		public string ZItemId
		{
			get { return Entry.ZItemId; }
			set { Entry.ZItemId = value; }
		}

		public BookType Type
		{
			get { return Entry.Type; }
			set { Entry.Type = value; }
		}

		public string Title
		{
			get { return Entry.Title; }
			set { Entry.Title = value; NotifyChanged( "Title" ); }
		}

		public string Description
		{
			get { return Entry.Description; }
			set { Entry.Description = value; NotifyChanged( "Description" ); }
		}

		public bool IsFav
		{
			get { return Entry.Fav; }
			set { Entry.Fav = value; NotifyChanged( "IsFav" ); }
		}

		public DateTime LastCache
		{
			get { return Entry.DateModified; }
			set { Entry.DateModified = value; NotifyChanged( "LastCache" ); }
		}

		public List<Volume> Volumes => Entry.Volumes;
		public BookInfo Info => Entry.Info;
		public HashSet<string> Others => Info.Others;

		private BitmapImage _Cover = null;

		protected string CoverUrl => FileLinks.ROOT_COVER + PathId + ".cvr";
		public bool CoverExist => Shared.Storage.FileExists( CoverUrl );
		public Stream CoverStream() => Shared.Storage.GetStream( CoverUrl );
		public ImageSource Cover => ( _Cover = _Cover ?? new BitmapImage() { UriSource = new Uri( "ms-appdata:///local/" + CoverUrl ) } );
		// Since the path are always the same but the underlying
		// file may change
		// We need a different object reference each time for
		// notifying changes
		public NameValue<string> CoverSourcePath => CoverExist ? new NameValue<string>( "Cover", CoverUrl ) : null;

		public string DailyHitCount => DisplayString( Info.DailyHitCount, PropType.DailyHitCount );
		public string TotalHitCount => DisplayString( Info.TotalHitCount, PropType.TotalHitCount );
		public string FavCount => DisplayString( Info.FavCount, PropType.FavCount );
		public string PushCount => DisplayString( Info.PushCount, PropType.PushCount );
		public string PostingDate => DisplayString( Info.PostingDate, PropType.PostingDate );
		public string LatestSection => DisplayString( Info.LatestSection, PropType.LatestSection );
		public string StatusLong => DisplayString( Status, PropType.Status );
		public string UpdateStatus => DisplayString( Info.LastUpdateDate, PropType.LastUpdateDate, "( " + Status + " ) " );
		public string Author => DisplayString( Info.Author, PropType.Author );
		public string Press => DisplayString( Info.Press, PropType.Press );
		public string Length => DisplayString( Info.Length, PropType.Length );

		virtual public bool NeedUpdate { get; protected set; }

		virtual public string LastUpdateDate
		{
			get { return DisplayString( Info.LastUpdateDate, PropType.LastUpdateDate ); }
			set
			{
				if ( Info.LastUpdateDate != value )
				{
					NeedUpdate = true;
					Info.LastUpdateDate = value;

					NotifyChanged( "LastUpdateDate", "UpdateStatus" );
				}
			}
		}

		public string Status
		{
			get
			{
				if ( int.TryParse( Info.Status, out int temp ) )
				{
					return ( temp == 0 ) ? Res.Text( "Status_Active" ) : Res.Text( "Status_Ended;" );
				}
				return Info.Status;
			}
			set { Info.Status = value; NotifyChanged( "Status", "UpdateStatus", "StatusLong" ); }
		}

		private string _IntroError;
		public string Intro
		{
			get
			{
				if ( !string.IsNullOrEmpty( _IntroError ) )
					return _IntroError;

				return string.IsNullOrEmpty( Info.LongDescription ) ? Entry.Description : Info.LongDescription;
			}
			set
			{
				Info.LongDescription = value;
				NotifyChanged( "Intro" );
			}
		}

		public string PlainTextInfo => string.Join(
			"\n"
			, $"{PropertyName( PropType.Title )}: {Title}"
			, $"Cover: {Info.CoverSrcUrl}"
			, Author, Press
			, TotalHitCount, DailyHitCount, PushCount, FavCount, Length
			, LastUpdateDate
			, PostingDate, LatestSection, StatusLong
			, string.Join( "\n", Others )
			, PropertyName( PropType.Intro ) + ": " + Intro
		);

		protected BookItem( Book Bk )
		{
			if ( Shared.BooksDb.SafeRun( x => x.Entry( Bk ).State == Microsoft.EntityFrameworkCore.EntityState.Detached ) )
			{
				if ( !Shared.BooksDb.SafeEntry( Bk ) )
					throw new InvalidOperationException( "Unsafe entry is not allowed" );
			}

			_Entry = Bk;
		}

		protected BookItem( string ZoneId, BookType SrcType, string ItemId )
		{
			_Entry = Shared.BooksDb.GetBook( ZoneId, ItemId, SrcType );

			if ( Entry.Id == 0 )
			{
				if ( GRConfig.ContentReader.IsHorizontal )
				{
					Entry.TextLayout = LayoutMethod.VerticalWriting;
				}

				if ( GRConfig.ContentReader.IsRightToLeft )
				{
					Entry.TextLayout = Entry.TextLayout | LayoutMethod.RightToLeft;
				}
			}
		}

		public void IntroError( string Msg )
		{
			_IntroError = Msg;
			NotifyChanged( "Intro" );
		}

		virtual public void Update( BookItem B )
		{
			if ( this == B || B == null )
				return;

			Entry.Title = B.Title;
			Entry.Description = B.Description;

			typeof( BookInfo )
				.GetProperties()
				.Where( x => x.GetType() == typeof( string ) )
				.ExecEach( x => x.SetValue( Entry.Info, x.GetValue( B.Entry.Info ) ) );

			Entry.Json_Meta = B.Entry.Json_Meta;
		}

		virtual public Volume[] GetVolumes() => Volumes.ToArray();

		virtual public void SaveInfo()
		{
			// Perform double check if this entry already exist in database
			if ( !Shared.BooksDb.Entry( Entry ).IsKeySet )
			{
				Book DbRecord = Shared.BooksDb.QueryBook( Type, ZoneId, ZItemId );
				if ( DbRecord != null )
				{
					DbRecord.Title = Entry.Title;
					DbRecord.Description = Entry.Description;
					DbRecord.Json_Meta = Entry.Json_Meta;
					DbRecord.Info = Entry.Info;

					Shared.BooksDb.RemoveUnsaved( Entry );
					_Entry = DbRecord;
				}
			}

			Shared.BooksDb.SaveBook( Entry );
		}

		public bool ReadParam( string Name, string Value, string CDATA = null )
		{
			if ( CDATA == null ) CDATA = Value;

			switch ( Name )
			{
				case AppKeys.XML_BINF_TITLE: Title = CDATA; break;
				case AppKeys.XML_BINF_AUTHOR:
					Info.Author = Value;
					NotifyChanged( "Author" );
					break;
				case AppKeys.XML_BMTA_PUSHCNT:
					Info.PushCount = Value;
					NotifyChanged( "PushCount" );
					break;
				case AppKeys.XML_BMTA_THITCNT:
					Info.TotalHitCount = Value;
					NotifyChanged( "TotalHitCount" );
					break;
				case AppKeys.XML_BMTA_DHITCNT:
					Info.DailyHitCount = Value;
					NotifyChanged( "DailyHitCount" );
					break;
				case AppKeys.XML_BMTA_FAVCNT:
					Info.FavCount = Value;
					NotifyChanged( "FavCount" );
					break;
				case AppKeys.BINF_DATE:
					Info.PostingDate = Value;
					NotifyChanged( "PostingDate" );
					break;
				case AppKeys.BINF_LUPDATE:
				case AppKeys.XML_BINF_LUPDATE:
					LastUpdateDate = Value;
					break;
				case AppKeys.XML_BMTA_LSECTION:
					Info.LatestSection = CDATA;
					NotifyChanged( "LatestSection" );
					break;
				case AppKeys.BINF_LENGTH:
				case AppKeys.XML_BMTA_BLENGTH:
					Info.Length = Value;
					NotifyChanged( "Length" );
					break;
				case AppKeys.BINF_PRESS:
				case AppKeys.XML_BMTA_PRESSID:
					Info.Press = Value;
					NotifyChanged( "Press" );
					break;
				case AppKeys.BINF_STATUS:
				case AppKeys.XML_BINF_BSTATUS:
					Status = Value;
					break;

				case AppKeys.XML_BINF_INTROPRV: Description = CDATA; break;
				case AppKeys.BINF_INTRO: Intro = Value; break;
				case AppKeys.BINF_ORGURL: Info.OriginalUrl = Value; break;
				// Special for spider, push content properties
				case AppKeys.BINF_OTHERS: Others.Add( Value ); break;

				case AppKeys.BINF_COVER:
					try
					{
						new Uri( Value );
						Info.CoverSrcUrl = Value;
						break;
					}
					catch ( Exception ) { }
					return false;
				default:
					return false;
			}

			return true;
		}

		public void SaveCover( byte[] Data )
		{
			string FileType = GSystem.Utils.GetMimeFromBytes( Data );
			if ( !( FileType.Contains( "jpeg" )
				|| FileType.Contains( "png" )
				|| FileType.Contains( "gif" )
				|| FileType.Contains( "bmp" ) ) )
			{
				Logger.Log( ID, "SaveCover: Invalid Data: " + FileType, LogType.WARNING );
				return;
			}

			Shared.Storage.WriteBytes( CoverUrl, Data );

			_Cover = null;
			NotifyChanged( "Cover", "CoverExist", "CoverSourcePath" );
		}

		public void ClearCover()
		{
			Shared.Storage.DeleteFile( CoverUrl );

			_Cover = null;
			NotifyChanged( "Cover", "CoverExist", "CoverSourcePath" );
		}

	}
}