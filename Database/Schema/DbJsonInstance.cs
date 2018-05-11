using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace GR.Database.Schema
{
	abstract public class DbJsonInstance
	{
		protected IJsonValue _Data;
		virtual public IJsonValue Data => _Data;

		public DbJsonInstance( IJsonValue Data )
		{
			_Data = Data;
		}
	}
}