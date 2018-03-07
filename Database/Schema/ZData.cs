using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

	}
}