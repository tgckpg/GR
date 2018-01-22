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
	using Database.Models;
	using ListItem;
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

		private static StringResBg _Res;
		private static StringResBg Res => _Res ?? ( _Res = new StringResBg( "Book" ) );

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

		private string CoverUrl => FileLinks.ROOT_COVER + Entry.Meta[ AppKeys.BINF_COVER ];
		public bool CoverExist => Entry.Meta.ContainsKey( AppKeys.BINF_COVER ) && Shared.Storage.FileExists( CoverUrl );
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
			, PostingDate, LatestSection, StatusLong
			, string.Join( "\n", Others )
			, PropertyName( PropType.Intro ) + ": " + Intro
		);

		protected BookItem( Book Bk )
		{
			if ( Shared.BooksDb.Entry( Bk ).State == Microsoft.EntityFrameworkCore.EntityState.Detached )
			{
				if ( Bk.Id == 0 ) throw new InvalidOperationException( "Only tracked Entry is allowed" );
			}

			_Entry = Bk;
		}

		protected BookItem( string ZoneId, BookType SrcType, string ItemId )
		{
			_Entry = Shared.BooksDb.GetBook( ZoneId, ItemId, SrcType );
		}

		public void IntroError( string Msg )
		{
			_IntroError = Msg;
			NotifyChanged( "Intro" );
		}

		public void Update( BookItem B )
		{
			Entry.Title = B.Title;
			Entry.Description = B.Description;
			Entry.Info = B.Info;
			Entry.Json_Meta = B.Entry.Json_Meta;
		}

		virtual public Volume[] GetVolumes() => Volumes.ToArray();

		virtual public void SaveInfo()
		{
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
			string ImageUid = GSystem.Utils.Md5( Data.AsBuffer() );

			Entry.Meta[ AppKeys.BINF_COVER ] = ImageUid;
			SaveInfo();

			Shared.Storage.WriteBytes( CoverUrl, Data );

			_Cover = null;
			NotifyChanged( "Cover", "CoverExist", "CoverSourcePath" );
		}

		public void ClearCover()
		{
			Shared.Storage.DeleteFile( CoverUrl );

			Entry.Meta.Remove( AppKeys.BINF_COVER );
			SaveInfo();

			_Cover = null;
			NotifyChanged( "Cover", "CoverExist", "CoverSourcePath" );
		}

	}
}