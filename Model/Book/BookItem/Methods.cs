using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Windows.UI.Xaml.Media.Imaging;

using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;

namespace GR.Model.Book
{
	using Database.Schema;
	using Effects;
	using Resources;
	using Settings;

	partial class BookItem
	{
		public static string TypeName( BookInfo InfoType )
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
			Shared.BooksDb.Books.Update( Entry );
			Shared.BooksDb.SaveChanges();
		}

		virtual public void SaveInfo( XRegistry XReg )
		{
			throw new NotSupportedException();
		}

		virtual public void ReadInfo( XRegistry XReg )
		{
			throw new NotSupportedException();
		}

		public void Update( BookItem B )
		{
			Id = B.Id;
			Title = B.Title;
			Author = B.Author;
			RecentUpdate = B.RecentUpdate;
			TotalHitCount = B.TotalHitCount;
			TodayHitCount = B.TodayHitCount;
			PushCount = B.PushCount;
			FavCount = B.FavCount;
			Length = B.Length;
			LatestSection = B.LatestSection;
			Press = B.Press;
			Intro = B.Intro;
			Info.OriginalUrl = B.Info.OriginalUrl;
			Entry.Info.Json_Others = B.Info.Json_Others;
		}

		public void CoverUpdate()
		{
			NotifyChanged( "CoverExistsPath" );
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

		private string DisplayString( string Raw, BookInfo InfType, string Suffix = "" )
		{
			return string.IsNullOrEmpty( Raw ) ? "" : ( TypeName( InfType ) + ": " + Raw + Suffix );
		}

		private async void TrySetSource()
		{
			if ( _Cover != null || CoverExistsPath == null ) return;

			using ( Stream s = Shared.Storage.GetStream( CoverPath ) )
			{
				_Cover = new BitmapImage();
				await _Cover.SetSourceAsync( s.AsRandomAccessStream() );

				NotifyChanged( "Cover" );
			}
		}

	}
}