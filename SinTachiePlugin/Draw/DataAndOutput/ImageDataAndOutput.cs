using System.IO;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin;

namespace SinTachiePlugin.Draw.DataAndOutput
{
    internal class ImageDataAndOutput : IDisposable
    {
        public readonly string FilePath;
        private readonly IImageFileSource? _source;
        public readonly int Width;
        public readonly int Height;
        public ID2D1Image? Output => _source?.Output;

        public ImageDataAndOutput(IGraphicsDevicesAndContext devices, string filePath)
        {
            if (Path.Exists(filePath) && ImageFileSourceFactory.Create(devices, filePath) is IImageFileSource source)
            {
                FilePath = filePath;
                _source = source;
                Width = source.Output.PixelSize.Width;
                Height = source.Output.PixelSize.Height;
            }
            else
            {
                FilePath = string.Empty;
            }
        }

        public void Dispose()
        {
            _source?.Dispose();
        }
    }
}
