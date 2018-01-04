using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

using Net.Astropenguin.Linq;

namespace GR.Database.Schema
{
	public class DbDictionary : Dictionary<string, string>
	{
		public string Data
		{
			get
			{
				JsonObject Obj = new JsonObject();
				this.ExecEach( x => Obj.Add( x.Key, x.Value == null ? JsonValue.CreateNullValue() : JsonValue.CreateStringValue( x.Value ) ) );
				return Obj.Stringify();
			}
			set
			{
				try
				{
					Clear();
					JsonObject.Parse( value ).ExecEach( x => this[ x.Key ] = x.Value.ValueType == JsonValueType.Null ? null : x.Value.GetString() );
				}
				catch ( Exception ex )
				{
					this[ "ERR" ] = ex.Message;
				}
			}
		}
	}
}