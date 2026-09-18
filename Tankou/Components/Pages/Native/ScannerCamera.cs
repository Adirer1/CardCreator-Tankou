using DrawnUi.Camera;
using Tankou.Core.Models;
using Tankou.Core.Services;
namespace Tankou.Components.Pages.Native
{
    public class ScannerCamera : SkiaCamera
    {
        FrameBuffer frameBuffer;
        RgbaFrame rgbaFrame;


        public FrameHandler? OCR { get; set; }

        readonly Lock bufferLock = new Lock(); //handles the concurrency problems

        readonly SemaphoreSlim frameAvailableSemaphore = new SemaphoreSlim(0, 1);


        CancellationTokenSource? loopState;

        public override void OnStateChanged(HardwareState state)
        {
            base.OnStateChanged(state);
            if(loopState != null)
            {
                loopState.Cancel();
                loopState.Dispose();
                loopState = null;
            }
        
            if (state == HardwareState.On)
            {
                FrameHandler? ocr = OCR;
                if (ocr != null)
                {
                    loopState = new CancellationTokenSource();
                    _ = ReadLoopAsync(ocr, loopState.Token);
                }
            }
        }
        protected override void OnRawFrameAvailable(RawCameraFrame frame)
        {
            base.OnRawFrameAvailable(frame);
            
            FillBuffer(frame);

        }



        private void FillBuffer(RawCameraFrame frame)
        {
            if (!bufferLock.TryEnter())
            {
                return;
            }
            try
            {
                frameBuffer.currentWidth = Math.Min(frame.SourceWidth, frame.SourceHeight);
                frameBuffer.currentHeight = Math.Max(frame.SourceWidth, frame.SourceHeight);

                int size = frameBuffer.currentWidth * frameBuffer.currentHeight * 4;
                if (frameBuffer.bufferArray == null || frameBuffer.bufferArray.Length != size)//checks if a buffer exists and if its size is correct, if not, it creates a new buffer and writes a debug message.
                {
                    frameBuffer.bufferArray = new byte[size];
                    System.Diagnostics.Debug.WriteLine($"frame buffer created: {frameBuffer.currentWidth} x {frameBuffer.currentHeight}");
                }

                if (!frame.TryGetRgba(frameBuffer.currentWidth, frameBuffer.currentHeight, frameBuffer.bufferArray, OutputOrientation.Display, 1f))// tries to copy the frame to the buffer, if unsuccessful, it writes a debug message and returns.
                {
                    System.Diagnostics.Debug.WriteLine("[Tankou] pixel copy failed");
                    return;
                }
                rgbaFrame = new RgbaFrame(frameBuffer.bufferArray, frameBuffer.currentWidth, frameBuffer.currentHeight);
                if (frameAvailableSemaphore.CurrentCount == 0)
                {
                    frameAvailableSemaphore.Release();
                }
            }
            finally
            {
                bufferLock.Exit();

            }

            
        }
        async Task ReadLoopAsync(FrameHandler ocr, CancellationToken token)
        {

            byte[]? readerPixels = null;

            await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding); // guarantees that the loop runs on a background thread and not on the UI thread

            try
            {
                while (true)
                {
                    await frameAvailableSemaphore.WaitAsync(token);
                    RgbaFrame copy;
                    lock (bufferLock)
                    {
                        if (readerPixels == null || readerPixels.Length != rgbaFrame.Pixels.Length)
                        {
                            readerPixels = new byte[rgbaFrame.Pixels.Length];
                        }
                        Array.Copy(rgbaFrame.Pixels, readerPixels, readerPixels.Length);
                        copy = new RgbaFrame(readerPixels, rgbaFrame.Width, rgbaFrame.Height);
                    }

                    await FrameReader.ReadFrameAsync(ocr, copy);

                }
                
            }
            catch(OperationCanceledException)
            {
              
            }
        }
    }
}
