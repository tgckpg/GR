using Microsoft.Graphics.Canvas;

namespace wenku8.Effects.P2DFlow
{
    interface IWireFrame
    {
        void WireFrame( CanvasDrawingSession ds );
        void FreeWireFrame();
    }
}