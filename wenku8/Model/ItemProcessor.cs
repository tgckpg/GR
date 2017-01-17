using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Model.Pages
{
    using Book;
    using Book.Spider;
    using Ext;
    using ListItem;
    using Settings;

    sealed class ItemProcessor
    {
        public static async Task ProcessLocal( LocalBook LB )
        {
            await LB.Process();
            if ( LB is SpiderBook )
            {
                SpiderBook SB = ( SpiderBook ) LB;
                BookInstruction BS = SB.GetBook();
                if ( BS.Packable )
                {
                    BS.PackVolumes( SB.GetPPConvoy() );
                }
            }
        }

        public static async Task<BookItem> GetBookFromId( string Id )
        {
            Guid _Guid;
            int _Id;

            bool IsBookSpider = false;
            IsBookSpider = Guid.TryParse( Id, out _Guid );

            if ( !IsBookSpider && Id.Contains( '/' ) )
            {
                string[] ZSId = Id.Split( '/' );
                IsBookSpider = ZSId.Length == 2 && ZSId[ 0 ][ 0 ] == AppKeys.SP_ZONE_PFX;
            }

            if( IsBookSpider )
            {
                SpiderBook Book = await SpiderBook.CreateAsyncSpider( Id );
                if( Book.ProcessSuccess ) return Book.GetBook();
            }
            else if( int.TryParse( Id, out _Id ) )
            {
                // Order-aware
                IDeathblow Deathblow = X.Instance<IDeathblow>( XProto.Deathblow, Id );
                if ( Deathblow.Registered && X.Exists )
                {
                    await ProcessLocal( Deathblow.GetParser() );
                    return Deathblow.GetBook();
                }

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
            BookItem B = X.Instance<BookItem>( XProto.BookItemEx, Id );
            B.XSetProp( "Mode", X.Const<string>( XProto.WProtocols, "ACTION_BOOK_META" ) );

            return B;
        }

    }
}