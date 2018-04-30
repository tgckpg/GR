using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Loaders
{
	using AdvDM;
	using Resources;
	using Settings;

	class TRTable
	{
		RuntimeCache RCache = new RuntimeCache();

		public bool Validate( string Type ) => Shared.Storage.FileExists( FileLinks.ROOT_WTEXT + "tr-" + Type );

		public async Task<byte[]> Get( string Type )
		{
			string Local = FileLinks.ROOT_WTEXT + "tr-" + Type;

			if ( Shared.Storage.FileExists( Local ) )
			{
				return Shared.Storage.GetBytes( Local );
			}

			byte[] Data = await Download( Type );
			if ( Data.Any() )
			{
				var j = Task.Run( () => Shared.Storage.WriteBytes( Local, Data ) );
			}

			return Data;
		}

		public Task<byte[]> Download( string Type )
		{
			TaskCompletionSource<byte[]> Bytes = new TaskCompletionSource<byte[]>();

			RCache.POST(
				Shared.ShRequest.Server
				, new PostData( Type, "action=tr-table&type=" + Type )
				, ( e, id ) => Bytes.TrySetResult( e.ResponseBytes )
				, ( c, Id, ex ) => Bytes.TrySetResult( new byte[ 0 ] )
				, false
			);

			return Bytes.Task;
		}

	}
}