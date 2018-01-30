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
		public string Error { get; private set; }
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
					Error = null;
					Clear();
					JsonObject.Parse( value ).ExecEach( x => this[ x.Key ] = x.Value.ValueType == JsonValueType.Null ? null : x.Value.GetString() );
				}
				catch ( Exception ex )
				{
					Error = ex.Message;
				}
			}
		}
	}

	public class DbDictionary<T> : Dictionary<string, T>
		where T : DbJsonInstance
	{
		public string Error { get; private set; }
		public string Data
		{
			get
			{
				JsonObject Obj = new JsonObject();
				this.ExecEach( x => Obj.Add( x.Key, x.Value == null ? JsonValue.CreateNullValue() : x.Value.Data ) );
				return Obj.Stringify();
			}
			set
			{
				try
				{
					Error = null;
					Clear();
					JsonObject.Parse( value ).ExecEach( x => this[ x.Key ] = ( T ) Activator.CreateInstance( typeof( T ), x.Value.ValueType == JsonValueType.Null ? null : x.Value ) );
				}
				catch ( Exception ex )
				{
					Error = ex.Message;
				}
			}
		}
	}
}