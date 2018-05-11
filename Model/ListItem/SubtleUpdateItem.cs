using System;
using Windows.UI;

namespace GR.Model.ListItem
{
	using Config;

	class SubtleUpdateItem : ActiveItem
	{
		private Color sc = GRConfig.Theme.SubtleColor;

		protected bool iNew = false;
		public bool IsNew { get { return iNew; } }
		public Type Nav { get; private set; }

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

		virtual public void SetNew( bool val )
		{
			if ( iNew = val )
			{
				SubtleColor = GRConfig.Theme.ColorMajor;
			}
			else
			{
				SubtleColor = GRConfig.Theme.SubtleColor;
			}
		}

		public SubtleUpdateItem( string Name, string Desc, string Desc2, string id )
			: base( Name, Desc, Desc2, id ) { }

		public SubtleUpdateItem( string Name, string UpdateDesc, Type Page, string id )
			: base( Name, UpdateDesc, id )
		{
			Nav = Page;
		}
	}
}