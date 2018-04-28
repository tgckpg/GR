using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Logging;


namespace GR.Model.Loaders
{
	using AdvDM;
	using Resources;
	using Settings;

	class TRTable
	{
		RuntimeCache RCache = new RuntimeCache();

		public async Task<byte[]> Get( string Type )
		{
			string Local = FileLinks.ROOT_WTEXT + "tr-" + Type;

			if ( Shared.Storage.FileExists( Local ) )
			{
				return Shared.Storage.GetBytes( Local );
			}

			TaskCompletionSource<byte[]> Bytes = new TaskCompletionSource<byte[]>();

			RCache.POST(
				Shared.ShRequest.Server
				, new PostData( Type, "action=tr-table&type=" + Type )
				, ( e, id ) =>
				{
					Bytes.TrySetResult( e.ResponseBytes );
				}
				, ( c, Id, ex ) =>
				{
					Bytes.TrySetResult( new byte[ 0 ] );
					Logger.Log( "TRTable", "Cannot get table" + Type, LogType.WARNING );
				}
				, false
			);

			byte[] Data = await Bytes.Task;
			var j = Task.Run( () => Shared.Storage.WriteBytes( Local, Data ) );

			return Data;
		}
	}
}