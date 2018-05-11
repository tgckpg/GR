using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.AdvDM
{
	using Model.Book;
	using Model.Interfaces;

	sealed class ImageService
	{
		public static IImageService GetProvider( BookItem Bk )
		{
			BingService BS = new BingService( Bk );
			if ( BS.Exists() )
				return BS;

			return new GoogleCustomSearch( Bk );
		}
	}
}