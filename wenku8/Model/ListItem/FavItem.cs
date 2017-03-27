using Windows.UI;
using Windows.UI.Xaml;
using System;
using System.Globalization;

using Net.Astropenguin.Loaders;
using Net.Astropenguin.UI;

namespace wenku8.Model.ListItem
{
	using Config;

	class FavItem : BookInfoItem 
	{
		private bool wSync = false, _autoCache = false, uActive = false;
		private Color sc = Properties.APPEARENCE_THEME_SUBTLE_TEXT_COLOR;

		private StringResources stx = new StringResources( "ContextMenu" );

		public string RSync
		{
			get
			{
				return wSync
					? stx.Text( "WUnsync" )
					: stx.Text( "WSync" )
					;
			}
		}

		public string AutoCacheText
		{
			get
			{
				return _autoCache
					? stx.Text( "DisAutomation" )
					: stx.Text( "AutoUpdate" )
					;
			}
			set
			{
				_autoCache = ( value == "1" );
				NotifyChanged( "AutoCache" );
			}
		}

		public bool AutoCache
		{
			get
			{
				return _autoCache;
			}
			set
			{
				_autoCache = value;
				NotifyChanged( "AutoCacheText", "AutoCache", "CacheState" );
			}
		}

		public Color SubtleColor
		{
			get
			{
				return sc;
			}
			set
			{
				sc = value;
				NotifyChanged( "SubtleColor" );
			}
		}

		public bool UpdateActive
		{
			get { return uActive; }
			set
			{
				uActive = value;
				NotifyChanged( "CacheState" );
			}
		}

		public bool wenkuSync
		{
			get { return wSync; }
			set
			{
				wSync = value;
				NotifyChanged( "RSync", "SyncState" );
			}
		}

		public ControlState SyncState
		{
			get { return wSync ? ControlState.Reovia : ControlState.Foreatii; }
		}

		public ControlState CacheState
		{
			get { return _autoCache ? ControlState.Reovia : ControlState.Foreatii; }
		}

		private DateTime _date = new DateTime( 0 );
		public DateTime Date { get { return _date; } }

		public FavItem( string Name, string Date, string LastUpdate, string id, bool wenkusync, bool Automation, bool isNew )
			: base( id, Name, LastUpdate, Date )
		{
			_autoCache = Automation;
			wenkuSync = wenkusync;

			Mode = SectionMode.DirectNavigation;

			SubtleColor = Properties.APPEARENCE_THEME_RELATIVE_SHADES_COLOR;
			SetNew( isNew );

			DateTime.TryParseExact( Date + "+8", "yyyy-MM-ddz", CultureInfo.CurrentCulture, DateTimeStyles.None, out _date );
		}

		public void SetNew( bool val )
		{
			// Default subtle color is not set here
			// The item marked is "NEW" should keep the status
			// Unless user explicitly acknowledge that
			if ( val ) SubtleColor = Properties.APPEARENCE_THEME_MAJOR_COLOR;
		}

		internal void Update( FavItem u )
		{
			if ( !string.IsNullOrEmpty( Desc ) )
			{
				SetNew( Desc != u.Desc );
			}

			Desc = u.Desc;
			Desc2 = u.Desc2;
			wenkuSync = u.wenkuSync;
		}
	}
}
