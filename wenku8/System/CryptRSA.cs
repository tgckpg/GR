using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace wenku8.System
{
    sealed class CryptRSA
    {
        public string Name { get; set; }

        private AsymmetricKeyAlgorithmProvider AsymKeyProvider = AsymmetricKeyAlgorithmProvider.OpenAlgorithm( AsymmetricAlgorithmNames.RsaSignPkcs1Sha512 );
        private CryptographicKey RSAKeyPair;

        public CryptRSA( string PublicKey, string PrivateKey = null )
        {
            if ( string.IsNullOrEmpty( PrivateKey ) )
            {
                RSAKeyPair = AsymKeyProvider.ImportPublicKey(
                    Convert.FromBase64String( PublicKey ).AsBuffer()
                );
            }
            else
            {
                RSAKeyPair = AsymKeyProvider.ImportKeyPair(
                    Convert.FromBase64String( PrivateKey ).AsBuffer()
                );
            }
        }

        public CryptRSA( uint Len = 256 )
        {
            RSAKeyPair = AsymKeyProvider.CreateKeyPair( 256 );
        }

        public string GenPublicKey()
        {
            return Convert.ToBase64String(
                RSAKeyPair.ExportPublicKey( CryptographicPublicKeyBlobType.X509SubjectPublicKeyInfo ).ToArray()
            );
        }

        public string GetPrivateKey()
        {
            return Convert.ToBase64String(
                RSAKeyPair.Export().ToArray()
            );
        }

        public string Encrypt( string Data )
        {
            IBuffer DataEnc = CryptographicBuffer.ConvertStringToBinary( Data, BinaryStringEncoding.Utf8 );
            return Encrypt( DataEnc );
        }

        public string Encrypt( IBuffer Data )
        {
            IBuffer EncBuffer = CryptographicEngine.Encrypt( RSAKeyPair, Data, null );
            return Convert.ToBase64String( EncBuffer.ToArray() );
        }

        public string Decrypt( string EncData )
        {
            return CryptographicBuffer.ConvertBinaryToString( BinaryStringEncoding.Utf8, DecryptToBuffer( EncData ) );
        }

        public IBuffer DecryptToBuffer( string EncData )
        {
            IBuffer Data = Convert.FromBase64String( EncData ).AsBuffer();
            IBuffer DecBuffer = CryptographicEngine.Decrypt( RSAKeyPair, Data, null );
            return DecBuffer;
        }
    }
}
