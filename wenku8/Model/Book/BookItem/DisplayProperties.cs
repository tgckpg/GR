using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wenku8.Resources;
using Windows.UI.Xaml.Media;

namespace wenku8.Model.Book
{
    partial class BookItem
    {
        private bool iv = false;
        public bool IsFav
        {
            get { return iv; }
            set { iv = value; NotifyChanged( "IsFav" ); }
        }

        private ImageSource CoverSource;
        public ImageSource Cover
        {
            get
            {
                return CoverSource;
            }
            set
            {
                CoverSource = value;
                NotifyChanged( "Cover" );
            }
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string LatestSection { get; set; }
        public string OriginalUrl { get; set; }
        public HashSet<string> Others { get; set; }


        private string TodayHitCountRaw = "";
        public string TodayHitCount
        {
            get { return DisplayString( TodayHitCountRaw, BookInfo.DailyHitsCount ); }
            set { TodayHitCountRaw = value; }
        }

        private string TotalHitCountRaw = "";
        public string TotalHitCount
        {
            get { return DisplayString( TotalHitCountRaw, BookInfo.TotalHitsCount ); }
            set { TotalHitCountRaw = value; }
        }


        private string FavCountRaw = "";
        public string FavCount
        {
            get { return DisplayString( FavCountRaw, BookInfo.FavCount ); }
            set { FavCountRaw = value; }
        }

        private string PushCountRaw = "";
        public string PushCount
        {
            get { return DisplayString( PushCountRaw, BookInfo.PushCount ); }
            set { PushCountRaw = value; NotifyChanged( "PushCount" ); }
        }

        // Assesed by BookInfoView for LocalBookStorage
        internal string RecentUpdateRaw { get; private set; }
        public string RecentUpdate
        {
            get { return DisplayString( RecentUpdateRaw, BookInfo.Date ); }
            set { RecentUpdateRaw = value; }
        }

        public string UpdateStatus
        {
            get { return DisplayString( RecentUpdateRaw, BookInfo.Date, "( " + Status + " ) " ); }
        }

        // Assesed by BookInfoView for Author Search
        internal string AuthorRaw { get; private set; }
        public string Author
        {
            get { return DisplayString( AuthorRaw, BookInfo.Author ); }
            set { AuthorRaw = value; }
        }

        private string PressRaw = "";
        public string Press
        {
            get { return DisplayString( PressRaw, BookInfo.Press ); }
            set { PressRaw = value; }
        }

        private string IntroRaw = "";
        public string Intro
        {
            get
            {
                return Shared.Storage.FileExists( IntroPath )
                  ? Shared.Storage.GetString( IntroPath )
                  : IntroRaw
                  ;
            }
            set
            {
                IntroRaw = value;
                NotifyChanged( "Intro" );
            }
        }

        private string st = "";
        public string Status
        {
            get
            {
                int temp;
                if ( !int.TryParse( st, out temp ) )
                    return st;
                return ( temp == 0 ) ? Res.Text( "Status_Active" ) : Res.Text( "Status_Ended;" );
            }
            set { st = value; }
        }
        public string StatusLong
        {
            get { return DisplayString( Status, BookInfo.Status ); }
        }

        private string LengthRaw = "";
        public string Length
        {
            get { return DisplayString( LengthRaw, BookInfo.Length ); }
            set { LengthRaw = value; }
        }

        public string PlainTextInfo
        {
            get
            {
                string Content = "";
                Content += TypeName( BookInfo.Title ) + ": " + Title;
                Content += "\nCover: " + CoverSrcUrl;
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
