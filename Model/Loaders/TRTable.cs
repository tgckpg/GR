using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Logging;

namespace GR.Model.Loaders
{
	using AdvDM;
	using Resources;
	using Settings;

	class TRTable
	{
		RuntimeCache _RCache;
		RuntimeCache RCache => _RCache ?? ( _RCache = new RuntimeCache() );

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

			if ( Shared.ShRequest == null )
			{
				// Since ShRequest instance is unavailable here
				// We do not download the tables on background tasks
				if ( !Worker.BackgroundOnly )
				{
					Logger.Log( "TRTable", "ShRequest is null!", LogType.WARNING );
				}

				Bytes.TrySetResult( new byte[ 0 ] );
			}
			else
			{
				RCache.POST(
					Shared.ShRequest.Server
					, new PostData( Type, "action=tr-table&type=" + Type )
					, ( e, id ) => Bytes.TrySetResult( e.ResponseBytes )
					, ( c, Id, ex ) => Bytes.TrySetResult( new byte[ 0 ] )
					, false
				);
			}

			return Bytes.Task;
		}

	}
}