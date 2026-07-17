using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Draw.DataAndOutput
{
    internal class SceneDataAndOutput : IDisposable
    {
        private readonly IGraphicsDevicesAndContext _devices;
        public readonly Guid SceneId;
        public TimeSpan Time;
        private readonly ISceneInfo? _targetScene;
        private readonly ITimelineSource? _source;
        public bool Occupied { get; set; } = false;
        public ID2D1Image? Output => _source?.Output;

        public SceneDataAndOutput(IGraphicsDevicesAndContext devices, TimelineItemSourceDescription desc, Guid sceneId, TimeSpan time, bool loop)
        {
            _devices = devices;
            SceneId = sceneId;
            _targetScene = desc.Scenes.FirstOrDefault(x => x.ID == sceneId);

            if (_targetScene is not null && _targetScene.TryCreateVideoSource(_devices, out _source))
            {
                Time = time;
                _source.Update(GetValidTimeSpan(time, loop), desc.Usage);
            }
            else
            {
                Time = TimeSpan.Zero;
            }
        }

        public TimeSpan GetValidTimeSpan(TimeSpan time, bool loop)
        {
            if (_targetScene is null)
                return TimeSpan.Zero;
            else if (time.Ticks > _targetScene.Duration.Time.Ticks && !loop)
                return TimeSpan.FromTicks(_targetScene.Duration.Time.Ticks - 1);
            else
                return TimeSpan.FromTicks(time.Ticks % _targetScene.Duration.Time.Ticks);
        }

        public void Update(TimelineItemSourceDescription desc, TimeSpan time)
        {
            if (_targetScene is not null && _source is not null)
            {
                Time = time;
                _source.Update(time, desc.Usage);
            }
            else
            {
                Time = TimeSpan.Zero;
            }
        }

        public void Dispose()
        {
            _source?.Dispose();
        }
    }
}
