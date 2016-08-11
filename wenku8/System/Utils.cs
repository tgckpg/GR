using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;

namespace wenku8.System
{
    class Utils
	{
		public static void DoNothing( string arg1, string arg2, Exception arg3 ) { }
		public static void DoNothing( DRequestCompletedEventArgs arg1, string arg2 ) { }
		public static void DoNothing() { }

		public static string AutoByteUnit( ulong size )
		{
            double b = 1.0d * size;
			string unit = "Byte";
			if ( b > 1024 )
			{
				b /= 1024;
				unit = "KB";
			}
			if ( b > 1024 )
			{
				b /= 1024;
				unit = "MB";
			}
			if ( b > 1024 )
			{
				b /= 1024;
				unit = "GB";
			}
			b = Math.Round( b, 2 );
			return b.ToString() + " " + unit;
		}

        public static bool Numberstring( string n )
        {
            if ( n.Length == 0 ) return false;
            foreach ( char p in n )
            {
                if ( !char.IsDigit( p ) )
                    return false;
            }
            return true;
        }

		public static DateTime GetDateTimeFromstring( string time )
		{
			if( Numberstring( time ) && time.Length != 14 )
				throw new FormatException();
			return new DateTime(
				int.Parse( time.Substring( 0, 4 ) )
				, int.Parse( time.Substring( 4, 2 ) )
				, int.Parse( time.Substring( 6, 2 ) )
				, int.Parse( time.Substring( 8, 2 ) )
				, int.Parse( time.Substring( 10, 2 ) )
				, int.Parse( time.Substring( 12 ) )
			 );
		}

		public static string GetStringStream( Stream e )
		{
			string p;
			using ( StreamReader k = new StreamReader( e ) )
			{
				p = k.ReadToEnd();
			}
			return p;
		}

        internal static void ShowError( Func<string> ErrorMessage )
        {
            Net.Astropenguin.Helpers.Worker.UIInvoke( async () =>
            {
                await Popups.ShowDialog( new MessageDialog( ErrorMessage() ) );
            } );
        }

        internal static bool CompareVersion( string thisVer, string CurrentVer )
		{
			string[] k = thisVer.Split( '.' );
			string[] l = CurrentVer.Split( '.' );
			if ( int.Parse( k[3] ) >= int.Parse( l[3] )
				&& int.Parse( k[2] ) >= int.Parse( l[2] )
				&& int.Parse( k[1] ) >= int.Parse( l[1] )
				&& int.Parse( k[0] ) >= int.Parse( l[0] )
				 ) return true;
			return false;
		}

        internal static string Md5( string str )
        {
            HashAlgorithmProvider alg = HashAlgorithmProvider.OpenAlgorithm( HashAlgorithmNames.Md5 );
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary( str, BinaryStringEncoding.Utf8 );
            return CryptographicBuffer.EncodeToHexString( alg.HashData( buff ) );
        }

        internal async static Task<string> Sha1( IStorageFile File )
        {
            HashAlgorithmProvider alg = HashAlgorithmProvider.OpenAlgorithm( HashAlgorithmNames.Sha1 );

            CryptographicHash hash = alg.CreateHash();
            BasicProperties Prop = await File.GetBasicPropertiesAsync();

            IBuffer buff = new Windows.Storage.Streams.Buffer( 1048576 );
            IRandomAccessStream rStream = await File.OpenAsync( FileAccessMode.Read );

            await rStream.ReadAsync( buff, 1048576, InputStreamOptions.None );
            while ( 0 < buff.Length )
            {
                hash.Append( buff );
                await rStream.ReadAsync( buff, 1048576, InputStreamOptions.None );
            }

            return CryptographicBuffer.EncodeToHexString( hash.GetValueAndReset() );
        }
    }
}
