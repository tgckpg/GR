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
	public class DbList : List<string>
	{
		public string Error { get; private set; }
		public string Data
		{
			get
			{
				JsonArray Arr = new JsonArray();
				this.ExecEach( x => Arr.Add( JsonValue.CreateStringValue( x ) ) );
				return Arr.Stringify();
			}
			set
			{
				try
				{
					Error = null;
					Clear();
					JsonArray.Parse( value ).GetArray().ExecEach( x => Add( x.GetString() ) );
				}
				catch ( Exception ex )
				{
					Error = ex.Message;
				}
			}
		}
	}

	public class DbList<T> : List<T>
		where T : DbJsonInstance
	{
		public string Error { get; private set; }
		public string Data
		{
			get
			{
				JsonArray Arr = new JsonArray();
				this.ExecEach( x => Arr.Add( x.Data ) );
				return Arr.Stringify();
			}
			set
			{
				try
				{
					Error = null;
					Clear();
					JsonArray.Parse( value ).GetArray().ExecEach( x => Add( ( T ) Activator.CreateInstance( typeof( T ), x ) ) );
				}
				catch ( Exception ex )
				{
					Error = ex.Message;
				}
			}
		}
	}
}