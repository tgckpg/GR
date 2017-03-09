using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace wenku8.Model.ListItem
{
	using Config;
	using Windows.UI.Xaml.Media.Imaging;

	sealed class NewsItem : Topic
	{
		public Uri Link { get; private set; }

		public bool IsNew { get { return LatestSection == "1"; } }

		public Uri Banner { get; private set; }

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
			: base( Title, Desc, Guid )
		{
			this.Link = new Uri( Link );

			TimeStamp = DateTime.Parse(
				DateStamp
				, CultureInfo.CurrentUICulture );

			try { ParseImage(); } catch ( Exception ) { }
		}

		private void ParseImage()
		{
			const string StartTok = "<img src=\"";
			const string EndTok = "\" />";

			if ( Desc.IndexOf( StartTok ) == 0 )
			{
				var j = Task.Run( () =>
				{
					int OffsetStart = StartTok.Length;
					int OffsetEnd = Desc.IndexOf( EndTok );

					string ImgSrc = WebUtility.HtmlDecode( Desc.Substring( OffsetStart, OffsetEnd - OffsetStart ) );

					Desc = Desc.Substring( OffsetEnd + EndTok.Length );

					Banner = new Uri( ImgSrc );

					NotifyChanged( "Banner" );
				} );
			}
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