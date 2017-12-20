using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Database.Schema
{
	public enum SqliteTriggers : byte
	{
		INSERT = 1, UPDATE = 2, DELETE = 4
	}

	public class AutoNow : Attribute 
	{
		public SqliteTriggers Triggers { get; set; }

		public AutoNow( SqliteTriggers Triggers )
		{
			this.Triggers = Triggers;
		}
	}
}
