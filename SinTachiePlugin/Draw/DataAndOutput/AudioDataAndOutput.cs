using System.IO;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.FileSource;

namespace SinTachiePlugin.Draw.DataAndOutput
{
    internal class AudioDataAndOutput : IDisposable
    {
        public readonly string FilePath;
        public readonly int TackIndex;
        public readonly TimeSpan Duration;
        public TimeSpan Time { get; private set; }
        private TimeSpan _nextTime;
        private readonly IAudioFileSource? _source;
        public bool Occupied { get; set; } = false;
        public float Value { get; private set; }
        private float[] _buf;

        public AudioDataAndOutput(string filePath, int trackIndex, int fluency)
        {
            if (Path.Exists(filePath) && AudioFileSourceFactory.Create(filePath, trackIndex) is IAudioFileSource source)
            {
                FilePath = filePath;
                TackIndex = trackIndex;
                _source = source;
                _buf = new float[fluency];
            }
            else
            {
                FilePath = string.Empty;
                _buf = [];
            }
        }

        public void Reset()
        {
            Occupied = false;
            Time = _nextTime;
        }

        public void Update(TimeSpan time, int fluency)
        {
            if (_source is null)
                return;

            if (fluency != _buf.Length)
                _buf = new float[fluency];

            Time = time;
            _source.Seek(time);
            _source.Read(_buf, 0, fluency);
            Value = _buf.Average();
            _nextTime = time + TimeSpan.FromSeconds(fluency / _source.Hz);
        }

        public void Dispose()
        {
            _source?.Dispose();
        }
    }
}
