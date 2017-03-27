using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

using Net.Astropenguin.DataModel;

namespace wenku8.System
{
	using Model.ListItem;

	sealed class CryptAES : NameValue<string>
	{
		private SymmetricKeyAlgorithmProvider SymKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm( SymmetricAlgorithmNames.AesCbcPkcs7 );
		private CryptographicKey Aes256CFB;

		public IBuffer KeyBuffer { get { return Base64Buffer( _Value ); } }

		public static string GenKey( uint Len = 256 )
		{
			return CryptographicBuffer.EncodeToBase64String( CryptographicBuffer.GenerateRandom( Len ) );
		}

		public static string RawBytes( string EncData )
		{
			int Dat = EncData.IndexOf( "\r\n" );
			return BitConverter.ToString( Convert.FromBase64String( EncData.Substring( Dat + 2 ) ) ).Replace( '-', ' ' );
		}

		public CryptAES( string Base64Key )
			: base( "", Base64Key )
		{
			Aes256CFB = SymKeyProvider.CreateSymmetricKey( KeyBuffer );
		}

		public IBuffer Base64Buffer( string Base64Str )
		{
			return CryptographicBuffer.DecodeFromBase64String( Base64Str );
		}

		public string Encrypt( string Data )
		{
			IBuffer DataEnc = CryptographicBuffer.ConvertStringToBinary( Data, BinaryStringEncoding.Utf8 );
			IBuffer iv = CryptographicBuffer.GenerateRandom( SymKeyProvider.BlockLength );

			IBuffer EncBuffer = CryptographicEngine.Encrypt( Aes256CFB, DataEnc, iv );
			return CryptographicBuffer.EncodeToBase64String( iv ) + "\r\n" + CryptographicBuffer.EncodeToBase64String( EncBuffer );
		}

		public string Decrypt( string EncData )
		{
			int Dat = EncData.IndexOf( "\r\n" );
			IBuffer iv = Base64Buffer( EncData.Substring( 0, Dat ) );
			IBuffer Data = Base64Buffer( EncData.Substring( Dat + 2 ) );

			IBuffer DecBuffer = CryptographicEngine.Decrypt( Aes256CFB, Data, iv );
			return CryptographicBuffer.ConvertBinaryToString( BinaryStringEncoding.Utf8, DecBuffer );
		}
	}
}