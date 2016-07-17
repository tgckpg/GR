using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Messaging;

namespace wenku8.Resources
{
    using CompositeElement;
    using Model.Book;
    using Model.REST;
    using Storage;

    static class Shared
    {
        // The general file storage
        public static GeneralStorage Storage;

        // Books Cache used by loaders
        public static BookPool BooksCache = new BookPool( 5 );

        // The default settings by locale
        public static LocaleDefaults LocaleDefaults = new LocaleDefaults();

        // Sharers Request
        public static SharersRequest ShRequest = new SharersRequest();

        private static bool IsTrad = Config.Properties.LANGUAGE_TRADITIONAL;

        public static string ToCTrad( this string v )
        {
            return IsTrad ? libtranslate.Chinese.Traditional( v ) : v;
        }

        private static StringResources LoadMesgRes;

        public static void LoadMessage( string MESG_ID, params string[] args )
        {
            Worker.UIInvoke( () =>
            {
                if ( LoadMesgRes == null ) LoadMesgRes = new StringResources( "LoadingMessage" );
                string mesg = LoadMesgRes.Str( MESG_ID );

                // Got the message, go back to background
                Task.Run( () =>
                {
                    mesg = string.IsNullOrEmpty( mesg ) ? MESG_ID : mesg;

                    if ( 0 < args.Length )
                    {
                        mesg = string.Format( mesg, args );
                    }

                    MessageBus.SendUI( new Message( typeof( LoadingMask ), mesg ) );
                } );

            } );
        }
    }
}
