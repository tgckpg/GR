using Microsoft.Graphics.Canvas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GR.Effects
{
	class TextureLoader : IDisposable
	{
		Dictionary<int,CanvasBitmap> ResPool;

		public CanvasBitmap this[ int key ] { get { return ResPool[ key ]; } }
		public TextureCenter Center;

		private Dictionary<string, int> KeyIndex;
		private volatile int _i = 0;

		public TextureLoader()
		{
			ResPool = new Dictionary<int,CanvasBitmap>();
			KeyIndex = new Dictionary<string, int>();
			Center = new TextureCenter();
		}

		public async Task<int> Load( ICanvasResourceCreator CC, string Key, string fname )
		{
			int i = 0;
			lock ( KeyIndex )
			{
				if ( KeyIndex.ContainsKey( Key ) )
				{
					// i = KeyIndex[ Key ];
					return KeyIndex[ Key ];
				}
				else
				{
					i = _i++;
					KeyIndex[ Key ] = i;
				}
			}

			CanvasBitmap CBmp = await CanvasBitmap.LoadAsync( CC, fname );

			Center[ i ] = new Vector2( ( float ) CBmp.Bounds.Width * 0.5f, ( float ) CBmp.Bounds.Height * 0.5f );
			ResPool[ i ] = CBmp;
			return i;
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