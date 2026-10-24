using System.IO;
using System.Numerics;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin;

namespace SinTachiePlugin.Draw.DataAndOutput
{
    internal class ImageDataAndOutput : IDisposable
    {
        public readonly string FilePath;
        private readonly IImageFileSource? _source;
        private readonly AffineTransform2D? _trans;
        public bool Used { get; set; } = false;
        public ID2D1Image? Output;

        public ImageDataAndOutput(IGraphicsDevicesAndContext devices, string filePath)
        {
            if (Path.Exists(filePath) && ImageFileSourceFactory.Create(devices, filePath) is IImageFileSource source)
            {
                FilePath = filePath;
                _source = source;
                _trans = new(devices.DeviceContext);
                _trans.SetInput(0, source.Output, true);
                _trans.TransformMatrix = Matrix3x2.CreateTranslation(
                    -source.Output.PixelSize.Width / 2.0f,
                    -source.Output.PixelSize.Height / 2.0f);
                Output = _trans.Output;
            }
            else
            {
                FilePath = string.Empty;
            }
        }

        public void Dispose()
        {
            if (_trans is not null)
            {
                _trans.SetInput(0, null, true);
                Output?.Dispose();
                _trans.Dispose();
            }

            if (_source is not null)
            {
                _source.Dispose();
            }
        }
    }
}
