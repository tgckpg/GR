﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Ext
{
	using Model.Book;

	interface IBookLoader
	{
		void Load( BookItem b, bool useCache );
		void LoadCover( BookItem b, bool useCache );
	}
}