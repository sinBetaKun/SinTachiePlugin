using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.ClippingArg;
using SinTachiePlugin.Part.LayerInformation.ClippingArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Clip;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.LoopPlayback;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Parent;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.PlaybackSpeed;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.SceneId;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.StartFrameNumber;
using SinTachiePlugin.Properties;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter
{
    internal class SceneParameter : SourceSelectArgBase, IParentParameter, ISceneIdParameter, IClipParameter, IPlaybackSpeedParameter, IStartFrameNumberParameter, ILoopPlaybackParameter
    {
        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_Parent), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string Parent { get => _parent; set => Set(ref _parent, value); }
        private string _parent = string.Empty;

        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_SceneId), ResourceType = typeof(TextResource))]
        [SceneComboBox]
        public Guid SceneId { get => _sceneId; set => Set(ref _sceneId, value); }
        private Guid _sceneId = Guid.Empty;

        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_ClippingMode), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public ClippingMode ClippingMode { get => _clippingMode; set => Set(ref _clippingMode, value); }
        private ClippingMode _clippingMode = ClippingMode.DontClip;

        [Display(AutoGenerateField = true)]
        public ClippingArgBase ClippingArg { get => _clippingArg; set => Set(ref _clippingArg, value); }
        private ClippingArgBase _clippingArg = new DontClipParameter();


        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_PlaybackSpeed), ResourceType = typeof(TextResource))]
        [TextBoxSlider("F2", "%", 0, 200)]
        [DefaultValue(100)]
        [Range(0, 99999)]
        public double PlaybackSpeed { get => _playbackSpeed; set => Set(ref _playbackSpeed, value); }
        private double _playbackSpeed = 1.0;

        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_StartFrameNumber), ResourceType = typeof(TextResource))]
        [FrameNumberEditor]
        [DefaultValue(0)]
        [Range(0, 99999)]
        public int StartFrameNumber { get => _startFrameNumber; set => Set(ref _startFrameNumber, value); }
        private int _startFrameNumber = 0;

        [Display(Name = nameof(TextResource.PartParam_LayerInfo_SourceSelectArg_LoopPlayback), ResourceType = typeof(TextResource))]
        [ToggleSlider]
        public bool LoopPlayback { get => _loopPlayback; set => Set(ref _loopPlayback, value); }
        private bool _loopPlayback = false;

        public SceneParameter()
        {
        }

        public SceneParameter(SharedDataStore? store) : base(store)
        {
        }

        public override ValueTask EndEditAsync()
        {
            ClippingArg = ClippingMode.Convert(ClippingArg);
            return base.EndEditAsync();
        }

        public override void CopyFrom(SourceSelectArgBase? origin)
        {
            if (origin is IParentParameter parentParameter)
                Parent = parentParameter.Parent;
            if (origin is ISceneIdParameter sceneParameter)
                SceneId = sceneParameter.SceneId;
            if (origin is IClipParameter clippingParameter)
            {
                ClippingMode = clippingParameter.ClippingMode;
                ClippingArg = clippingParameter.ClippingArg.GetClone();
            }
            if (origin is IPlaybackSpeedParameter playbackSpeedParameter)
                PlaybackSpeed = playbackSpeedParameter.PlaybackSpeed;
            if (origin is IStartFrameNumberParameter startFrameNumberParameter)
                StartFrameNumber = startFrameNumberParameter.StartFrameNumber;
            if (origin is ILoopPlaybackParameter loopPlaybackParameter)
                LoopPlayback = loopPlaybackParameter.LoopPlayback;
        }

        public override SourceSelectArgBase GetClone()
        {
            SceneParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [ClippingArg];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new ParentSharedData(this));
            store.Save(new SceneIdSharedData(this));
            store.Save(new ClipSharedData(this));
            store.Save(new PlaybackSpeedSharedData(this));
            store.Save(new StartFrameNumberSharedData(this));
            store.Save(new LoopPlaybackSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<ParentSharedData>() is ParentSharedData parentSharedData)
                parentSharedData.CopyTo(this);
            if (store.Load<SceneIdSharedData>() is SceneIdSharedData sceneIdSharedData)
                sceneIdSharedData.CopyTo(this);
            if (store.Load<ClipSharedData>() is ClipSharedData clipSharedData)
                clipSharedData.CopyTo(this);
            if (store.Load<PlaybackSpeedSharedData>() is PlaybackSpeedSharedData playbackSpeedSharedData)
                playbackSpeedSharedData.CopyTo(this);
            if (store.Load<StartFrameNumberSharedData>() is StartFrameNumberSharedData startFrameNumberSharedData)
                startFrameNumberSharedData.CopyTo(this);
            if (store.Load<LoopPlaybackSharedData>() is LoopPlaybackSharedData loopPlaybackSharedData)
                loopPlaybackSharedData.CopyTo(this);
        }
    }
}
