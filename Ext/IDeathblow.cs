using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Ext
{
	using Model.Book;
	using Model.ListItem;

	interface IDeathblow
	{
		string Id { get; }
		bool Check( byte[] responseBytes );

		LocalBook GetParser();
	}
}