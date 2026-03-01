using Vortice;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
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
        public ID2D1Image? Output => _source?.Output;

        public SceneDataAndOutput(IGraphicsDevicesAndContext devices, TimelineItemSourceDescription desc, Guid sceneId, TimeSpan time, bool loop)
        {
            _devices = devices;
            SceneId = sceneId;
            _targetScene = desc.Scenes.FirstOrDefault(x => x.ID == sceneId);

            if (_targetScene is not null && _targetScene.TryCreateVideoSource(_devices, out _source))
            {
                Time = time;

                if (time.Ticks > _targetScene.Duration.Time.Ticks && !loop)
                    _source.Update(TimeSpan.FromTicks(Math.Min(time.Ticks, _targetScene.Duration.Time.Ticks)), desc.Usage);
                else
                    _source.Update(TimeSpan.FromTicks(time.Ticks % _targetScene.Duration.Time.Ticks), desc.Usage);
            }
        }

        public void Update(TimelineItemSourceDescription desc, TimeSpan time, bool loop)
        {
            if (_targetScene is not null && _source is not null)
            {
                Time = time;

                if (time.Ticks > _targetScene.Duration.Time.Ticks && !loop)
                    _source.Update(TimeSpan.FromTicks(Math.Min(time.Ticks, _targetScene.Duration.Time.Ticks)), desc.Usage);
                else
                    _source.Update(TimeSpan.FromTicks(time.Ticks % _targetScene.Duration.Time.Ticks), desc.Usage);
            }
        }

        public void Dispose()
        {
            _source?.Dispose();
        }
    }
}
