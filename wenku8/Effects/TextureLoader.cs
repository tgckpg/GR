using Microsoft.Graphics.Canvas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Effects
{
    class TextureLoader : IDisposable
    {
        Dictionary<int,CanvasBitmap> ResPool;

        public CanvasBitmap this[ int key ] { get { return ResPool[ key ]; } }
        public TextureCenter Center;

        public TextureLoader()
        {
            ResPool = new Dictionary<int,CanvasBitmap>();
            Center = new TextureCenter();
        }

        public async Task Load( ICanvasResourceCreator CC, int i, string File )
        {
            CanvasBitmap CBmp = await CanvasBitmap.LoadAsync( CC, File );

            Center[ i ] = new Vector2( ( float ) CBmp.Bounds.Width * 0.5f, ( float ) CBmp.Bounds.Height * 0.5f );
            ResPool[ i ] = CBmp ;
        }

        public void Dispose()
        {
            foreach( KeyValuePair<int, CanvasBitmap> Res in ResPool )
            {
                Res.Value.Dispose();
            }

            ResPool.Clear();
        }

        public class TextureCenter
        {
            Dictionary<int, Vector2> VCenter = new Dictionary<int, Vector2>();

            public Vector2 this[ int key ]
            {
                get { return VCenter[ key ]; }
                set { VCenter[ key ] = value; }
            }
        }
    }
}