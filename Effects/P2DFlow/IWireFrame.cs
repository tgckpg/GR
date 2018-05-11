using Microsoft.Graphics.Canvas;

namespace GR.Effects.P2DFlow
{
	interface IWireFrame
	{
		void WireFrame( CanvasDrawingSession ds );
		void FreeWireFrame();
	}
}