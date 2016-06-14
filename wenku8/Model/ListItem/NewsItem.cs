using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wenku8.Config;
using Windows.Data.Html;

namespace wenku8.Model.ListItem
{
    class NewsItem : Topic 
    {
        public Uri Link { get; private set; }

        public bool IsNew { get { return LatestSection == "1"; } }

        public DateTime TimeStamp { get; private set; }

        public Windows.UI.Color FG
        {
            get
            {
                return IsNew
                    ? Properties.APPEARENCE_THEME_MAJOR_COLOR
                    : Properties.APPEARENCE_THEME_TEXT_COLOR_RELATIVE_TO_BACKGROUND
                    ;
            }
        }

        public NewsItem( string Title, string Desc, string Link, string Guid, string DateStamp )
            :base( Title, HtmlUtilities.ConvertToText( Desc ), Guid )
        {
            this.Link = new Uri( Link );

            TimeStamp = DateTime.Parse(
                DateStamp
                , CultureInfo.CurrentUICulture );
        }

        public void FlagAsNew()
        {
            LatestSection = "1";
            NotifyChanged( "IsNew", "FG" );
        }

        public void FlagAsRead()
        {
            LatestSection = "0";
            NotifyChanged( "IsNew", "FG" );
        }
    }
}
