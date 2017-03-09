using System;

namespace wenku8.Model.ListItem.Sharers
{
	sealed class Activity : NameValue<Action>
	{
		public DateTime TimeStamp { get; set; }
		public Activity( string Name, Action Value )
			: base( Name, Value )
		{
		}
	}
}