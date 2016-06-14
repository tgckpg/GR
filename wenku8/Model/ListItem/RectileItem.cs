namespace wenku8.Model.ListItem
{
    using CompositeElement;
    using Effects;

    class RectileItem : BookInfoItem
	{
        private bool _isLoaded = false;

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set
            {
                _isLoaded = true;
                NotifyChanged( "IsLoaded" );
            }
        }

        private RectileSize _size = RectileSize.Large;
        public RectileSize Size
        {
            get { return _size; }
            set
            {
                _size = value;
                NotifyChanged( "Size" );
            }
        }

        public double Rotation { get; set; }

        public RectileItem( string aid, string Title, string Intro, string Date, string BannerPath )
			: base( aid, Title, Intro, Date, BannerPath )
		{
            Rotation = AnimationTimer.RandDouble( 360 );
		}

		public RectileItem( string aid, string Title, string Intro, string Date )
			: base( aid, Title, Intro, Date )
		{
            Rotation = AnimationTimer.RandDouble( 360 );
		}
	}
}
