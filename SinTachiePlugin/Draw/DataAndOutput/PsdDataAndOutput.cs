using PsdParser;
using System.Collections.Immutable;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin.FileSource.Psd;

namespace SinTachiePlugin.Draw.DataAndOutput
{
    internal class PsdDataAndOutput : IDisposable
    {
        public readonly string FilePath;
        public ImmutableList<string> EnableLayers;
        private readonly IGraphicsDevicesAndContext _devices;
        private readonly PsdFolder _psdFolder;
        public readonly int Width;
        public readonly int Height;
        public bool Used { get; set; } = false;
        public ID2D1Bitmap? Output;

        public PsdDataAndOutput(string filePath, IGraphicsDevicesAndContext devices, PsdFile psdFile, IEnumerable<string> enableLayers)
        {
            FilePath = filePath;
            EnableLayers = [.. enableLayers];
            _devices = devices;
            _psdFolder = PsdFolder.Parse(psdFile);
            Width = psdFile.Header.Width;
            Height = psdFile.Header.Height;
            _psdFolder.SetEnableItems(EnableLayers);
            Output = PsdFileSourcePlugin.CreateBitmap(devices, _psdFolder, Width, Height);
        }

        public void SetLayers(IEnumerable<string> enableLayers)
        {
            ImmutableList<string> list = [.. enableLayers];
            
            if (!EnableLayers.SequenceEqual(list))
            {
                EnableLayers = list;
                _psdFolder.SetEnableItems(EnableLayers);
                Output?.Dispose();
                Output = PsdFileSourcePlugin.CreateBitmap(_devices, _psdFolder, Width, Height);
            }
        }

        public void Dispose()
        {
            Output?.Dispose();
        }
    }
}
