using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.AdvDM
{
	sealed class PostData
	{
		public string Id;

		public Stream DataStream { get; private set; }

		public string CacheName { get; private set; }
		public string LogStamp { get; private set; }

		public string Data
		{
			get
			{
				if ( StringData != null )
				{
					return StringData;
				}

				using ( StreamReader SR = new StreamReader( DataStream ) )
				{
					return SR.ReadToEnd();
				}
			}
		}

		private string StringData;

		public PostData( string Name, string Id, string Data )
			:this( Id, Data )
		{
			LogStamp = Name + ": " + Id;
		}

		public PostData( string Name, string Id, Stream Data )
			:this( Id, Data )
		{
			LogStamp = Name + ": " + Id;
		}

		public PostData( string Id, string Data )
		{
			this.Id = Id;
			LogStamp = Id;

			CacheName = GSystem.Utils.Md5( Id );
			StringData = Data;

#if DEBUG
			LogStamp += ", " + Data;
#endif
		}

		public PostData( string Id, Stream Data )
		{
			this.Id = Id;
			LogStamp = Id;

			CacheName = GSystem.Utils.Md5( Id );
			DataStream = Data;
		}
	}
}
