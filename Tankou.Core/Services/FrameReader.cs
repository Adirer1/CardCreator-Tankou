
using System.Diagnostics;
using Tankou.Core.Models;
namespace Tankou.Core.Services
{
    public static class FrameReader
    {
       
       
        public static async Task ReadFrameAsync(FrameHandler reader, RgbaFrame frame)
        {

            try
            {
                await reader.Invoke(frame);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Tankou] frame reader exception: {ex}");
            }
           

        }

       
    }
}