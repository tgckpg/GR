using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Pages
{
	using Book;
	using Book.Spider;
	using Ext;
	using ListItem;
	using Resources;
	using Settings;

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
			Guid _Guid;
			int _Id;

			bool IsBookSpider = false;
			IsBookSpider = Guid.TryParse( Id, out _Guid );

			if ( IsBookSpider )
			{
				SpiderBook Book = await SpiderBook.CreateSAsync( Id );
				if ( Book.ProcessSuccess ) return Book.GetBook();
			}
			else if ( int.TryParse( Id, out _Id ) )
			{
				// Order-aware
				IDeathblow DeathBlow = await GetDeathblow( Id );
				if ( DeathBlow != null ) return DeathBlow.GetBook();

				LocalTextDocument Doc = new LocalTextDocument( Id );
				if ( Doc.IsValid ) return Doc;

				if ( X.Exists ) return GetBookEx( Id );
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

			return null;
		}

		public static BookItem GetBookEx( string Id )
		{
			BookItem B = Shared.BooksCache[ Id ] ?? X.Instance<BookItem>( XProto.BookItemEx, Id );
			B.XSetProp( "Mode", X.Const<string>( XProto.WProtocols, "ACTION_BOOK_META" ) );

			return B;
		}

		public static async Task<IDeathblow> GetDeathblow( string Id )
		{
			if ( X.Exists )
			{
				IDeathblow Deathblow = X.Instance<IDeathblow>( XProto.Deathblow, Id );
				if ( Deathblow.Registered )
				{
					await ProcessLocal( Deathblow.GetParser() );
					return Deathblow;
				}
			}

			return null;
		}

	}
}