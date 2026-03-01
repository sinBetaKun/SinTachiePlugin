using System.IO;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.FileSource;

namespace SinTachiePlugin.Draw.DataAndOutput
{
    internal class VideoDataAndOutput : IDisposable
    {
        public readonly string FilePath;
        public TimeSpan Time { get; private set; }
        private IVideoFileSource? _source;
        public ID2D1Image? Output => _source?.Output;

        public VideoDataAndOutput(IGraphicsDevicesAndContext devices, string filePath, TimeSpan time, bool loop)
        {
            if (Path.Exists(filePath) && VideoFileSourceFactory.Create(devices, filePath) is IVideoFileSource source)
            {
                FilePath = filePath;
                Time = time;
                _source = source;

                if (time.Ticks > _source.Duration.Ticks && !loop)
                    _source.Update(TimeSpan.FromTicks(Math.Min(time.Ticks, _source.Duration.Ticks)));
                else
                    _source.Update(TimeSpan.FromTicks(time.Ticks % _source.Duration.Ticks));
            }
            else
            {
                FilePath = string.Empty;
            }
        }

        public void Update(TimeSpan time, bool loop)
        {
            if (_source is null)
                return;

            Time = time;

            if (time.Ticks > _source.Duration.Ticks && !loop)
                _source.Update(TimeSpan.FromTicks(Math.Min(time.Ticks, _source.Duration.Ticks)));
            else
                _source.Update(TimeSpan.FromTicks(time.Ticks % _source.Duration.Ticks));
        }

        public void Dispose()
        {
            _source?.Dispose();
        }
    }
}
