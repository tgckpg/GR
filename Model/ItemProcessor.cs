using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Pages
{
	using Database.Models;
	using Book;
	using Book.Spider;
	using Ext;
	using ListItem;
	using Resources;

	sealed class ItemProcessor
	{
		public static async Task ProcessLocal( LocalBook LB )
		{
			await LB.Process();
			if ( LB is SpiderBook SB )
			{
				BookInstruction Inst = SB.GetBook();
				if ( Inst.Packable )
				{
					Inst.PackVolumes( SB.GetPPConvoy() );
				}
			}
		}

		public static async Task<BookItem> GetBookFromId( string Id )
		{
			bool IsBookSpider = false;
			IsBookSpider = Guid.TryParse( Id, out Guid _Guid );

			if ( IsBookSpider )
			{
				SpiderBook Book = await SpiderBook.CreateSAsync( Id );
				if ( Book.ProcessSuccess ) return Book.GetBook();
			}
			else if ( int.TryParse( Id, out int _Id ) )
			{
				if ( X.Exists ) return GetBookEx( Id );

				LocalTextDocument Doc = new LocalTextDocument( Id );
				if ( Doc.IsValid ) return Doc;
			}

			return null;
		}

		public static BookItem GetBookItem( Book Bk )
		{
			if ( X.Exists && Bk.Type.HasFlag( BookType.W ) )
			{
				return X.Call<BookItem>( XProto.BookItemEx, "Create", Bk );
			}

			switch( Bk.Type )
			{
				case BookType.S:
					return new BookInstruction( Bk );
				case BookType.L:
					return new LocalTextDocument( Bk );
			}

			return null;
		}

		public static Task<BookItem> GetBookFromTileCmd( string Command )
		{
			string[] Cmd = Command.Split( '|' );

			if ( Cmd.Length == 2 && !string.IsNullOrEmpty( Cmd[ 1 ] ) )
			{
				return GetBookFromId( Cmd[ 1 ] );
			}
			else if ( Cmd.Length == 3 )
			{
				return Task.Run( () =>
				{
					BookType BType = ( BookType ) Enum.Parse( typeof( BookType ), Cmd[ 0 ] );
					Book Bk = Shared.BooksDb.QueryBook( BType, Cmd[ 1 ], Cmd[ 2 ] );

					if ( Bk == null )
						return null;

					return GetBookItem( Bk );
				} );
			}

			return null;
		}

		public static BookItem GetBookEx( string Id )
		{
			BookItem B = X.Call<BookItem>( XProto.BookItemEx, "GetItem", Id );
			B.XSetProp( "Mode", X.Const<string>( XProto.WProtocols, "ACTION_BOOK_META" ) );

			return B;
		}

	}
}