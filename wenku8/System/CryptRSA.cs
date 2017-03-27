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
	sealed class CryptRSA : INamable
	{
		public string Name { get; set; }

		private AsymmetricKeyAlgorithmProvider AsymKeyProvider = AsymmetricKeyAlgorithmProvider.OpenAlgorithm( AsymmetricAlgorithmNames.RsaPkcs1 );
		private CryptographicKey RSAKeyPair;

		public CryptRSA( string PublicKey, string PrivateKey = null )
		{
			if ( string.IsNullOrEmpty( PrivateKey ) )
			{
				RSAKeyPair = AsymKeyProvider.ImportPublicKey(
					CryptographicBuffer.DecodeFromBase64String( PublicKey )
				);
			}
			else
			{
				RSAKeyPair = AsymKeyProvider.ImportKeyPair(
					CryptographicBuffer.DecodeFromBase64String( PrivateKey )
				);
			}
		}

		public CryptRSA()
		{
			// Today is 26 July 2016
			// Assuming this app or I am still around in 2030
			// but technology is evolving fast, it might probably be shorter.
			// 14 years huh, I wonder how many traces were there left?
			RSAKeyPair = AsymKeyProvider.CreateKeyPair( 4096 );
		}

		public string GenPublicKey()
		{
			return CryptographicBuffer.EncodeToBase64String(
				RSAKeyPair.ExportPublicKey( CryptographicPublicKeyBlobType.X509SubjectPublicKeyInfo )
			);
		}

		public string GetPrivateKey()
		{
			return CryptographicBuffer.EncodeToBase64String(
				RSAKeyPair.Export()
			);
		}

		/// <summary>
		/// Encrypt string
		/// </summary>
		/// <param name="Data">Data to encript</param>
		/// <param name="DeBase64">Decode the Data from base64 encoding before encrypting it</param>
		/// <returns></returns>
		public string Encrypt( string Data, bool DeBase64 = true )
		{
			IBuffer DataEnc = DeBase64
				? CryptographicBuffer.DecodeFromBase64String( Data )
				: CryptographicBuffer.ConvertStringToBinary( Data, BinaryStringEncoding.Utf8 )
				;
			return Encrypt( DataEnc );
		}

		public string Encrypt( IBuffer Data )
		{
			IBuffer EncBuffer = CryptographicEngine.Encrypt( RSAKeyPair, Data, null );
			return CryptographicBuffer.EncodeToBase64String( EncBuffer );
		}

		/// <summary>
		/// Decrypt string
		/// </summary>
		/// <param name="EncData">The encrypted data</param>
		/// <param name="EnBase64">Encodes the decrypted result to base64 string</param>
		/// <returns></returns>
		public string Decrypt( string EncData, bool EnBase64 = true )
		{
			IBuffer Data = CryptographicBuffer.DecodeFromBase64String( EncData );
			return EnBase64
				? CryptographicBuffer.EncodeToBase64String( Decrypt( Data ) )
				: CryptographicBuffer.ConvertBinaryToString( BinaryStringEncoding.Utf8, Decrypt( Data ) )
				;
		}

		public IBuffer Decrypt( IBuffer Data )
		{
			IBuffer DecBuffer = CryptographicEngine.Decrypt( RSAKeyPair, Data, null );
			return DecBuffer;
		}
	}
}