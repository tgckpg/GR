using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Book
{
	using Database.Models;
	using Resources;

	sealed class FTSResult
	{
		public int ChapterId { get; private set; }

		public string Title { get; set; }
		public string VolTitle { get; set; }
		public string EpTitle { get; set; }
		public string Result { get; set; }

		public FTSResult( int ChapterId, string Snippet )
		{
			this.ChapterId = ChapterId;
			Result = Snippet;

			Chapter C = Shared.BooksDb.Chapters.Find( ChapterId );
			if ( C != null )
			{
				EpTitle = C.Title;
				VolTitle = Shared.BooksDb.Volumes.Where( x => x.Id == C.VolumeId ).Select( x => x.Title ).FirstOrDefault();
				Title = Shared.BooksDb.Books.Where( x => x.Id == C.BookId ).Select( x => x.Title ).FirstOrDefault();
			}
		}
	}
}