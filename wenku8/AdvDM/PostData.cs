using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.AdvDM
{
    sealed class PostData
    {
        public string Name;

        public string CacheName { get; private set; }

        public string Data
        {
            get
            {
                if ( StringData != null )
                {
                    return StringData;
                }

                using ( StreamReader SR = new StreamReader( StreamData ) )
                {
                    return SR.ReadToEnd();
                }
            }
        }

        public Stream DataStream
        {
            get
            {
                return StreamData;
            }
        }

        private string StringData;
        private Stream StreamData;

        public PostData( string Name, string Data )
        {
            this.Name = Name;
            CacheName = wenku8.System.Utils.MD5( Name );
            StringData = Data;
        }

        public PostData( string Name, Stream Data )
        {
            this.Name = Name;
            CacheName = wenku8.System.Utils.MD5( Name );
            StreamData = Data;
        }
    }
}
