using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml.Media.Imaging;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.IO;
using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;

namespace GR.Model.Book
{
	using Effects;
	using Resources;
	using Settings;

	enum PropType
	{
		Others = 1,
		Title = 2,
		DailyHitsCount = 4,
		TotalHitsCount = 8,
		FavCount = 16,
		PushCount = 32,
		Date = 64,
		Author = 128,
		Press = 256,
		Status = 512,
		Length = 1024,
		LatestSection = 2048,
		Cover = 4096,
		Intro = 8192,
	}

	partial class BookItem : IActiveData
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyChanged( params string[] Names )
		{
			if ( Worker.BackgroundOnly ) return;

			Worker.UIInvoke( () =>
			{
				// Must check each time after property changed is called
				// PropertyChanged may be null after event call
				foreach ( string Name in Names )
				{
					PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( Name ) );
				}
			} );
		}

		public int Index { get; protected set; }

		virtual public bool NeedUpdate { get; protected set; }

		private static StringResBg Res { get { return new StringResBg( "Book" ); } }

		public static string TypeName( PropType InfoType )
		{
			return Res.Text( InfoType.ToString() );
		}

		public void IntroError( string Msg )
		{
			_IntroError = Msg;
			NotifyChanged( "Intro" );
		}

		public bool ParseXml( string xml )
		{
			try
			{
				XDocument BookmetaXml = XDocument.Parse( xml );
				IEnumerable<XElement> metadata = BookmetaXml.FirstNode.Document.Descendants( "data" );
				foreach ( XElement md in metadata )
				{
					ReadParam( md.GetXValue( "name" ), md.GetXValue( "value" ), md.Value );
				}
			}
			catch ( Exception )
			{
				return false;
			}
			return true;
		}

		public bool ReadParam( string Name, string Value, string CDATA = null )
		{
			if ( CDATA == null ) CDATA = Value;

			switch ( Name )
			{
				case AppKeys.XML_BINF_TITLE: Title = CDATA; return true;
				case AppKeys.XML_BINF_AUTHOR: Author = Value; return true;
				case AppKeys.XML_BMTA_PUSHCNT: PushCount = Value; return true;
				case AppKeys.XML_BMTA_THITCNT: TotalHitCount = Value; return true;
				case AppKeys.XML_BMTA_DHITCNT: TodayHitCount = Value; return true;
				case AppKeys.XML_BMTA_FAVCNT: FavCount = Value; return true;
				case AppKeys.XML_BMTA_LSECTION: LatestSection = CDATA; return true;
				case AppKeys.XML_BINF_INTROPRV: Description = CDATA; return true;
				case AppKeys.BINF_INTRO: Intro = Value; return true;
				case AppKeys.BINF_ORGURL: Info.OriginalUrl = Value; return true;
				// Special for spider, push content properties
				case AppKeys.BINF_OTHERS: Others.Add( Value ); return true;

				case AppKeys.BINF_COVER:
					try
					{
						new Uri( Value );
						Info.CoverSrcUrl = Value;
						return true;
					}
					catch ( Exception )
					{

					}
					return false;
				case AppKeys.BINF_PRESS:
				case AppKeys.XML_BMTA_PRESSID:
					Press = Value;
					return true;
				case AppKeys.BINF_STATUS:
				case AppKeys.XML_BINF_BSTATUS:
					Status = Value;
					return true;
				case AppKeys.BINF_LENGTH:
				case AppKeys.XML_BMTA_BLENGTH:
					Length = Value;
					return true;
				case AppKeys.BINF_DATE:
				case AppKeys.XML_BINF_LUPDATE:
					RecentUpdate = Value;
					return true;
			}

			return false;
		}

		virtual public void ParseVolumeData( string Data ) { }
		virtual public Database.Models.Volume[] GetVolumes() => Volumes.ToArray();

		virtual public void SaveInfo()
		{
			Shared.SaveBook( Entry );
		}

		public void Update( BookItem B )
		{
			Entry.Title = B.Title;
			Entry.Description = B.Description;
			Entry.Info = B.Info;
			Entry.Json_Meta = B.Entry.Json_Meta;
		}

		static public BookItem DummyBook()
		{
			BookItem Book = new NonCollectedBook( "Dummy Title" );
			Book.Author = "\u659F\u914C \u9D6C\u5144";
			Book.RecentUpdate = DateTime.Now.ToString( "yyyy-MM-dd" );
			Book.TotalHitCount = NTimer.RandInt( 0, 10000000 ).ToString();
			Book.TodayHitCount = NTimer.RandInt( 0, 10000000 ).ToString();
			Book.PushCount = NTimer.RandInt( 0, 10000000 ).ToString();
			Book.FavCount = NTimer.RandInt( 0, 10000000 ).ToString();
			Book.Length = NTimer.RandInt( 0, 10000000 ).ToString();
			Book.LatestSection = "The last savior";
			Book.Press = "Blog";
			Book.Intro = "Ducimus architecto qui sit sint odit ut.Nemo dolor minima sapiente. In reprehenderit qui voluptas voluptatibus.In in at voluptatem qui et dolor.Vitae natus consequatur autem sit autem. Enim asperiores quis soluta enim quos eveniet nobis qui."
				+ "\nEum eos sapiente voluptatem. Expedita hic at pariatur repellat.Praesentium dolorem eos quasi voluptatibus optio distinctio ea. Ea modi qui quam sapiente.Debitis enim facere odit dolor impedit. Tempore et quia fugiat hic atque nostrum neque earum."
				+ "\nVitae consequuntur ducimus aut dolore repellat sint.Ab quos dolores facere. Et sit ea rerum aut minima. Fuga sequi iure sunt tempore quia error dolorem. Nulla modi distinctio sit corrupti et et omnis laboriosam.Qui est ratione nesciunt et officia.";

			Book.Info.OriginalUrl = "https://blog.astropenguin.net/";

			Book.Others.Add( "Others 1" );
			Book.Others.Add( "Others 2" );
			Book.Others.Add( "Others 3" );

			return Book;
		}

		private string DisplayString( string Raw, PropType InfType, string Suffix = "" )
		{
			return string.IsNullOrEmpty( Raw ) ? "" : ( TypeName( InfType ) + ": " + Raw + Suffix );
		}

		private string CoverUrl => FileLinks.ROOT_COVER + Entry.Id;
		public bool CoverExist => Shared.Storage.FileExists( CoverUrl );
		public Stream CoverStream() => Shared.Storage.GetStream( CoverUrl );

		public void SaveCover( byte[] Data )
		{
			Shared.Storage.WriteBytes( CoverUrl, Data );
			_Cover = null;
			NotifyChanged( "Cover", "CoverSourcePath" );
		}

		public void ClearCover()
		{
			Shared.Storage.DeleteFile( CoverUrl );
			_Cover = null;
			NotifyChanged( "Cover", "CoverSourcePath" );
		}

	}
}