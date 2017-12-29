using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace GR.Model.Book
{
	partial class BookItem
	{
		private BitmapImage _Cover = null;
		public ImageSource Cover
		{
			get
			{
				TrySetSource();
				return _Cover;
			}
		}

		private bool iv = false;
		public bool IsFav
		{
			get { return iv; }
			set { iv = value; NotifyChanged( "IsFav" ); }
		}

		public DateTime LastCache
		{
			get { return DateModified; }
			set { DateModified = value; NotifyChanged( "LastCache" ); }
		}

		public HashSet<string> Others { get; set; }

		public string TodayHitCount
		{
			get { return DisplayString( Info.TodayHitCount, BookInfo.DailyHitsCount ); }
			set { Info.TodayHitCount = value; NotifyChanged( "TodayHitCount" ); }
		}

		public string TotalHitCount
		{
			get { return DisplayString( Info.TotalHitCount, BookInfo.TotalHitsCount ); }
			set { Info.TotalHitCount = value; NotifyChanged( "TotalHitCount" ); }
		}

		public string FavCount
		{
			get { return DisplayString( Info.FavCount, BookInfo.FavCount ); }
			set { Info.FavCount = value; NotifyChanged( "FavCount" ); }
		}

		public string PushCount
		{
			get { return DisplayString( Info.PushCount, BookInfo.PushCount ); }
			set { Info.PushCount = value; NotifyChanged( "PushCount" ); }
		}

		// Assesed by BookInfoView for LocalBookStorage
		public string RecentUpdate
		{
			get { return DisplayString( Info.RecentUpdate, BookInfo.Date ); }
			set { Info.RecentUpdate = value; NotifyChanged( "RecentUpdate", "UpdateStatus" ); }
		}

		public string LatestSection
		{
			get { return DisplayString( Info.LatestSection, BookInfo.LatestSection ); }
			set { Info.LatestSection = value; NotifyChanged( "LatestSection" ); }
		}

		public string UpdateStatus
		{
			get { return DisplayString( Info.RecentUpdate, BookInfo.Date, "( " + Status + " ) " ); }
		}

		// Accesed by BookInfoView for Author Search
		public string Author
		{
			get { return DisplayString( Info.Author, BookInfo.Author ); }
			set { Info.Author = value; NotifyChanged( "Author" ); }
		}

		// Accessed by BookInfoView for ScirptUpload
		public string Press
		{
			get { return DisplayString( Info.Press, BookInfo.Press ); }
			set { Info.Press = value; NotifyChanged( "Press" ); }
		}

		private string _IntroError;
		public string Intro
		{
			get
			{
				if ( !string.IsNullOrEmpty( _IntroError ) )
					return _IntroError;

				return string.IsNullOrEmpty( Info.LongDescription ) ? Description : Info.LongDescription;
			}
			set
			{
				Info.LongDescription = value;
				NotifyChanged( "Intro" );
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
			set { Info.Status = value; NotifyChanged( "UpdateStatus", "Status", "StatusLong" ); }
		}
		public string StatusLong
		{
			get { return DisplayString( Status, BookInfo.Status ); }
		}

		public string Length
		{
			get { return DisplayString( Info.Length, BookInfo.Length ); }
			set { Info.Length = value; NotifyChanged( "Length" ); }
		}

		public string PlainTextInfo
		{
			get
			{
				string Content = "";
				Content += TypeName( BookInfo.Title ) + ": " + Title;
				Content += "\nCover: " + Info.CoverSrcUrl;
				Content += "\n" + Author;
				Content += "\n" + RecentUpdate;
				Content += "\n" + TotalHitCount;
				Content += "\n" + TodayHitCount;
				Content += "\n" + PushCount;
				Content += "\n" + FavCount;
				Content += "\n" + Length;
				Content += "\n" + LatestSection;
				Content += "\n" + Press;
				Content += "\n" + string.Join( "\n", Others );
				Content += "\n" + StatusLong;
				Content += "\n" + TypeName( BookInfo.Intro ) + ": " + Intro;

				return Content; 
			}
		}
	}
}