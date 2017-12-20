using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Loaders;

namespace GR.Ext
{
	using Model.Book;
	interface IListLoader : ILoader<BookItem>
	{
		int TotalCount { get; }
	}
}
