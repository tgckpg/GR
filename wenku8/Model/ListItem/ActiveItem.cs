using Net.Astropenguin.DataModel;

namespace wenku8.Model.ListItem
{
	class ActiveItem : ActiveData
	{
        private string _desc;
		public string Desc
		{
			get { return _desc; }
			set
            {
                _desc = value;
                NotifyChanged( "Desc" );
            }
		}

		public string Desc2
		{
			get;
			protected set;
		}

        private string _name;
		public string Name
		{
			get { return _name; }
			set
            {
                _name = value;
                NotifyChanged( "Name" );
            }
		}

		public string Payload
		{
			get;
			protected set;
		}

        public object RawPayload
        {
            get;
            protected set;
        }

        public ActiveItem( string Name, string Description, string Payload )
            : this( Name, Description, null, Payload )
        {
        }

        public ActiveItem( string Name, string Description, object Payload )
            : this( Name, Description, null, Payload.ToString() )
        {
            this.RawPayload = Payload;
        }

		public ActiveItem( string Name, string UDescription, string Description, string Payload )
		{
			this.Name = Name;
			Desc = UDescription;
			Desc2 = Description;
			this.Payload = Payload;
		}
	}
}
