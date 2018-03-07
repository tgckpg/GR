using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace GR.Model.REST
{
	sealed class JsonStatus
	{
		public static JsonObject Parse( string JsonData )
		{
			JsonObject JResponse;
			if ( !JsonObject.TryParse( JsonData, out JResponse ) )
			{
				throw new Exception( "A server Error has occurred" );
			}

			if ( !JResponse.GetNamedBoolean( "status", false ) )
			{
				throw new Exception( JResponse.GetNamedString( "message" ) );
			}

			return JResponse;
		}
	}
}