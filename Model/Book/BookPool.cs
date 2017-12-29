using System;
using System.Linq;
using System.Collections.Generic;

using Net.Astropenguin.Linq;
using Net.Astropenguin.Logging;


namespace GR.Model.Book
{
	class BookPool : List<BookItem>
	{
		public static readonly string ID = typeof( BookPool ).Name;

		private Dictionary<string, BookItem> Books;

		new public BookItem this[ int i ]
		{
			get
			{
				int j = 0;
				return Books.FirstOrDefault( x => j++ == i ).Value;
			}
			set
			{
				throw new ArgumentException( "BookItems cannot be set directly using int" );
			}
		}

		public BookItem[] AllBooks()
		{
			int l, i = 0;
			BookItem[] bList = new BookItem[ l = this.Count() ];

			foreach ( BookItem b in Books.Remap( x => x.Value ) )
			{
				if ( b != null )
					bList[i++] = b;
			}

			return bList;
		}

		public BookItem this[ string id ]
		{
			get
			{
				return ( Books.ContainsKey( id ) ? Books[ id ] : null );
			}

			set
			{
				// Unique id for books
				if ( Books.ContainsKey( id ) )
				{
					Logger.Log( ID, "Triggered Once: id( " + id + " ) ", LogType.INFO );
					// If found, update book info
					BookItem b = Books[ id ];
					b.TodayHitCount = value.TodayHitCount;
					b.TotalHitCount = value.TotalHitCount;
					b.PushCount = value.PushCount;
					b.FavCount = value.FavCount;
					b.RecentUpdate = value.RecentUpdate;
					b.Author = value.Author;
					b.Press = value.Press;
					b.Status = value.Status;
					b.Length = value.Length;
				}
				else
				{
					Books.Add( id, value );
				}
			}
		}

		public BookPool( int NumCallsGC )
		{
			Books = new Dictionary<string, BookItem>();
		}

		new public int Count()
		{
			return Books.Count();
		}

		public int IndexOf( string s )
		{
			int j = -1;

			if( Books.ContainsKey( s ) )
			{
				Books.Any( x => {
					j++;
					return x.Value.ZItemId == s;
				} );
			}

			return j;
		}
	}
}
