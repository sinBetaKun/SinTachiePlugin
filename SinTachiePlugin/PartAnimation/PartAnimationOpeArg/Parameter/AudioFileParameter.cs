using NAudio.Wave;
using SinTachiePlugin.Draw;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.Abrir;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.AudioFilePath;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.Cerrar;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.ContentOffset;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.PlaybackLoop;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.PlaybackSpeed;
using SinTachiePlugin.Properties;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter
{
    internal class AudioFileParameter : PartAnimationOpeArgBase, IAbrirParameter, ICerrarParameter, IAudioFilePathParameter, IContentOffsetParameter, IPlaybackSpeedParameter, IPlaybackLoopParameter
    {
        [Display(Name = nameof(TextResource.PartAnimationOpeArg_Open), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Abrir { get; } = new Animation(100, -10000, 10000);

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_Close), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Cerrar { get; } = new Animation(0, -10000, 10000);

        [Display(Name = nameof(TextResource.PartAnimationOpeMode_AudioFile), ResourceType = typeof(TextResource))]
        [FileSelector(YukkuriMovieMaker.Settings.FileGroupType.AudioItem)]
        public string AudioFilePath { get => _audioFilePath; set => Set(ref _audioFilePath, value); }
        private string _audioFilePath = string.Empty;

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_Fluency), ResourceType = typeof(TextResource))]
        [TextBoxSlider("F0", "", 0.0, 10.0)]
        [Range(0.0, 100000.0)]
        [DefaultValue(0.0)]
        public int Fluency { get => _fluency; set => Set(ref _fluency, value); }
        private int _fluency = 0;

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_StartFrameNumber), ResourceType = typeof(TextResource))]
        [TextBoxSlider("F0", "", 0.0, 3.0)]
        [Range(0.0, 100000.0)]
        [DefaultValue(0.0)]
        public int AudioTrackIndex { get => _audioTrackIndex; set => Set(ref _audioTrackIndex, value); }
        private int _audioTrackIndex = 0;

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_StartFrameNumber), ResourceType = typeof(TextResource))]
        [TimeSpanRange]
        [TimeSpanDefaultValue]
        [TimeSpanEditor]
        public TimeSpan ContentOffset { get => _contentOffset; set => Set(ref _contentOffset, value); }
        private TimeSpan _contentOffset = TimeSpan.Zero;

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_PlaybackSpeed), ResourceType = typeof(TextResource))]
        [TextBoxSlider("F2", "%", 0, 200)]
        [DefaultValue(100)]
        [Range(0, 99999)]
        public double PlaybackSpeed { get => _playbackSpeed; set => Set(ref _playbackSpeed, value); }
        private double _playbackSpeed = 100;

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_PlaybackLoop), ResourceType = typeof(TextResource))]
        [ToggleSlider]
        public bool PlaybackLoop { get => _playbackLoop; set => Set(ref _playbackLoop, value); }
        private bool _playbackLoop = false;

        private float _volume;

        public AudioFileParameter()
        {
        }

        public AudioFileParameter(SharedDataStore? store) : base(store)
        {
        }

        public void SetAudioVolume(float volume)
        {
            _volume = volume;
        }

        public override PartAnimationResultA GetResult(TachieSourceDescription desc)
        {
            FrameAndLength fl = new(desc);
            double open = fl.GetValue(Abrir, desc.FPS);
            double close = fl.GetValue(Cerrar, desc.FPS);

            return PartAnimationResultA.FromVolume(close + (open - close) * _volume);
        }

        public override void CopyFrom(PartAnimationOpeArgBase? origin)
        {
            if (origin is IAbrirParameter abrirParam)
                Abrir.CopyFrom(abrirParam.Abrir);
            if (origin is ICerrarParameter cerrarParam)
                Cerrar.CopyFrom(cerrarParam.Cerrar);
            if (origin is IAudioFilePathParameter audioFilePathParam)
                AudioFilePath = audioFilePathParam.AudioFilePath;
            if (origin is IContentOffsetParameter startFrameNumberParam)
                ContentOffset = startFrameNumberParam.ContentOffset;
            if (origin is IPlaybackSpeedParameter playbackSpeedParameter)
                PlaybackSpeed = playbackSpeedParameter.PlaybackSpeed;
            if (origin is IPlaybackLoopParameter playbackLoopParameter)
                PlaybackLoop = playbackLoopParameter.PlaybackLoop;
        }

        public override PartAnimationOpeArgBase GetClone()
        {
            AudioFileParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Abrir, Cerrar];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new AbrirSharedData(this));
            store.Save(new CerrarSharedData(this));
            store.Save(new AudioFilePathSharedData(this));
            store.Save(new ContentOffsetSharedData(this));
            store.Save(new PlaybackSpeedSharedData(this));
            store.Save(new PlaybackLoopSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<AbrirSharedData>() is AbrirSharedData abrirSharedData)
                abrirSharedData.CopyTo(this);
            if (store.Load<CerrarSharedData>() is CerrarSharedData cerrarSharedData)
                cerrarSharedData.CopyTo(this);
            if (store.Load<AudioFilePathSharedData>() is AudioFilePathSharedData audioFilePathSharedData)
                audioFilePathSharedData.CopyTo(this);
            if (store.Load<ContentOffsetSharedData>() is ContentOffsetSharedData startTimeSpanSharedData)
                startTimeSpanSharedData.CopyTo(this);
            if (store.Load<PlaybackSpeedSharedData>() is PlaybackSpeedSharedData playbackSpeedSharedData)
                playbackSpeedSharedData.CopyTo(this);
            if (store.Load<PlaybackLoopSharedData>() is PlaybackLoopSharedData playbackLoopSharedData)
                playbackLoopSharedData.CopyTo(this);
        }
    }
}
