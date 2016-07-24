﻿using System;
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
    sealed class CryptAES
    {
        public string Name { get; set; }

        private SymmetricKeyAlgorithmProvider SymKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm( SymmetricAlgorithmNames.AesCbcPkcs7 );
        private CryptographicKey Aes256CFB;

        public static string GenKey( uint Len = 256 )
        {
            return Convert.ToBase64String( CryptographicBuffer.GenerateRandom( Len ).ToArray() );
        }

        public CryptAES( string Base64Key )
        {
            Aes256CFB = SymKeyProvider.CreateSymmetricKey( Base64Buffer( Base64Key ) );
        }

        public IBuffer Base64Buffer( string Base64Str )
        {
            return Convert.FromBase64String( Base64Str ).AsBuffer();
        }

        public string Encrypt( string Data )
        {
            IBuffer DataEnc = CryptographicBuffer.ConvertStringToBinary( Data, BinaryStringEncoding.Utf8 );
            IBuffer iv = CryptographicBuffer.GenerateRandom( SymKeyProvider.BlockLength );

            IBuffer EncBuffer = CryptographicEngine.Encrypt( Aes256CFB, DataEnc, iv );
            return Convert.ToBase64String( iv.ToArray() ) + "\r\n" + Convert.ToBase64String( EncBuffer.ToArray() );
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