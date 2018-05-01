using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;

namespace GR.Database.Schema
{
	public class ZData
	{
		public byte[] RawBytes { get; set; }

		public string StringValue
		{
			get => Encoding.UTF8.GetString( Inflate() );
			set => Deflate( Encoding.UTF8.GetBytes( value ) );
		}

		public byte[] BytesValue
		{
			get => Inflate();
			set => Deflate( value );
		}

		private byte[] Inflate()
		{
			if ( RawBytes == null )
				return new byte[ 0 ];

			using ( MemoryStream s = new MemoryStream( RawBytes ) )
			using ( DeflateStream ZStream = new DeflateStream( s, CompressionMode.Decompress ) )
			using ( MemoryStream DataOut = new MemoryStream() )
			{
				ZStream.CopyTo( DataOut );
				return DataOut.ToArray();
			}
		}

		private void Deflate( byte[] bytes )
		{
			using ( MemoryStream ZOut = new MemoryStream() )
			{
				using ( MemoryStream s = new MemoryStream( bytes ) )
				using ( DeflateStream ZStream = new DeflateStream( ZOut, CompressionLevel.Optimal, true ) )
				{
					s.CopyTo( ZStream );
				}

				ZOut.Seek( 0, SeekOrigin.Begin );
				RawBytes = ZOut.ToArray();
			}
		}

		public string GetBase64Raw() => CryptographicBuffer.EncodeToBase64String( RawBytes.AsBuffer() );
		public void SetBase64Raw( string Data ) => RawBytes = CryptographicBuffer.DecodeFromBase64String( Data ).ToArray();
	}
}