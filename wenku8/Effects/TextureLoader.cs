using Microsoft.Graphics.Canvas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Effects
{
    class TextureLoader : IDisposable
    {
        Dictionary<int,CanvasBitmap> ResPool;

        public CanvasBitmap this[ int key ]
        {
            get
            {
                return ResPool[ key ];
            }
        }

        public TextureLoader()
        {
            ResPool = new Dictionary<int,CanvasBitmap>();
        }

        public async Task Load( ICanvasResourceCreator CC, int i, string File )
        {
            ResPool[ i ] = await CanvasBitmap.LoadAsync( CC, File );
        }

        public void Dispose()
        {
            foreach( KeyValuePair<int, CanvasBitmap> Res in ResPool )
            {
                Res.Value.Dispose();
            }

            ResPool.Clear();
        }
    }
}
