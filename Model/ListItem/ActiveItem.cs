using Net.Astropenguin.DataModel;

namespace GR.Model.ListItem
{
	class ActiveItem : ActiveData, INamable
	{
		private string _desc;
		virtual public string Desc
		{
			get { return _desc; }
			set
			{
				_desc = value;
				NotifyChanged( "Desc" );
			}
		}

		private string _desc2;
		virtual public string Desc2
		{
			get { return _desc2; }
			set
			{
				_desc2 = value;
				NotifyChanged( "Desc2" );
			}
		}

		private string _name;
		virtual public string Name
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

		protected ActiveItem() { }

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