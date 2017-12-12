using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Ext
{
	using Model.Book;

	interface IBookLoader
	{
		void Load( BookItem b, bool useCache );
	}
}