using System;
using Windows.UI;

namespace wenku8.Model.ListItem
{
    using Config;

    class SubtleUpdateItem : ActiveItem
	{
		private Color sc = Properties.APPEARENCE_THEME_SUBTLE_TEXT_COLOR;

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
				SubtleColor = Properties.APPEARENCE_THEME_MAJOR_COLOR;
			}
			else
			{
				SubtleColor = Properties.APPEARENCE_THEME_SUBTLE_TEXT_COLOR;
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
