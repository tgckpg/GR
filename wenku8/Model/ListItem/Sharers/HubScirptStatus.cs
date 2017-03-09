using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

using Net.Astropenguin.DataModel;

namespace wenku8.Model.ListItem.Sharers
{
	sealed class HubScriptStatus : ActiveData
	{
		public int Status { get; private set; }
		public DateTime Date { get; private set; }
		public string Desc { get; private set; }

		public string DescBR
		{
			get { return string.IsNullOrEmpty( Desc ) ? "" : "\n" + Desc; }
		}

		public HubScriptStatus( JsonObject JObject )
		{
			Status = ( int ) JObject.GetNamedNumber( "status" );
			Desc = JObject.GetNamedString( "desc" );
			Date = DateTime.Parse( JObject.GetNamedString( "date" ) );
		}
	}
}