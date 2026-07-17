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
        private readonly IVideoFileSource? _source;
        public bool Occupied { get; set; } = false;
        public ID2D1Image? Output => _source?.Output;

        public VideoDataAndOutput(IGraphicsDevicesAndContext devices, string filePath, TimeSpan time, bool loop)
        {
            if (Path.Exists(filePath) && VideoFileSourceFactory.Create(devices, filePath) is IVideoFileSource source)
            {
                FilePath = filePath;
                _source = source;
                TimeSpan t0 = GetValidTimeSpan(time, loop);
                Time = t0;
                _source.Update(t0);
            }
            else
            {
                FilePath = string.Empty;
            }
        }

        public TimeSpan GetValidTimeSpan(TimeSpan time, bool loop)
        {
            if (_source is null)
                return TimeSpan.Zero;
            else if (time.Ticks > _source.Duration.Ticks && !loop)
                return TimeSpan.FromTicks(_source.Duration.Ticks - 1);
            else
                return TimeSpan.FromTicks(time.Ticks % _source.Duration.Ticks);
        }

        public void Update(TimeSpan time)
        {
            if (_source is null)
                return;

            Time = time;
            _source.Update(time);

        }

        public void Dispose()
        {
            _source?.Dispose();
        }
    }
}
