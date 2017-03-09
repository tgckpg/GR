using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace wenku8.Model.ListItem
{
	sealed class PinRecord : TreeItem
	{
		public string DevId { get; private set; }
		public string Id { get; private set; }

		public string TileId { get; private set; }
		public bool TileExists { get; private set; }

		public DateTime Date { get; private set; }

		public PinRecord( string Id, string DeviceId, string Name, string TileId, DateTime Date )
			: base( Name, 1 )
		{
			this.Date = Date;
			this.Id = Id;
			this.TileId = TileId;
			DevId = DeviceId;

			if ( !string.IsNullOrEmpty( TileId ) )
			{
				TileExists = SecondaryTile.Exists( TileId );
			}
		}

		public PinRecord( string DeviceName, string DeviceId )
			: base( DeviceName, 0 )
		{
			DevId = DeviceId;
		}
	}
}