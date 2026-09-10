using DrawnUi.Camera;
using Tankou.Core.Models;
namespace Tankou.Components.Pages.Native
{
    public class ScannerCamera : SkiaCamera
    {
        byte[]? frameBuffer;

        public event EventHandler<RgbaFrame>? FrameReady;

        protected override void OnRawFrameAvailable(RawCameraFrame frame)
        {
            base.OnRawFrameAvailable(frame);
            int width = Math.Min(frame.SourceWidth, frame.SourceHeight);
            int height = Math.Max(frame.SourceWidth, frame.SourceHeight);

            int size = width * height * 4;
            if (frameBuffer == null || frameBuffer.Length != size)
            {
                frameBuffer = new byte[size];
                System.Diagnostics.Debug.WriteLine($"frame buffer created: {width} x {height}");
            }
        }
    }
}
