using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.ClippingArg;
using SinTachiePlugin.Part.LayerInformation.ClippingArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Clip;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.LoopPlayback;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Parent;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.PlaybackSpeed;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Point;
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
    internal class SceneParameter : SourceSelectArgBase, IParentParameter, ISceneIdParameter, IClipParameter, IPointParameter, IPlaybackSpeedParameter, IStartFrameNumberParameter, ILoopPlaybackParameter
    {
        [Display(Name = nameof(Texts.PartParam_LayerInfo_SourseSelectArg_Parent), ResourceType = typeof(Texts))]
        [TextEditor]
        public string Parent { get => parent; set => Set(ref parent, value); }
        private string parent = string.Empty;

        [Display(Name = nameof(Texts.PartParam_LayerInfo_SourseSelectArg_SceneId), ResourceType = typeof(Texts))]
        [SceneComboBox]
        public Guid SceneId { get => sceneId; set => Set(ref sceneId, value); }
        private Guid sceneId = Guid.Empty;

        [Display(Name = nameof(Texts.PartParam_LayerInfo_SourseSelectArg_ClippingMode), ResourceType = typeof(Texts))]
        [EnumComboBox]
        public ClippingMode ClippingMode { get => clippingMode; set => Set(ref clippingMode, value); }
        private ClippingMode clippingMode = ClippingMode.DontClip;

        [Display(AutoGenerateField = true)]
        public ClippingArgBase ClippingArg { get => clippingArg; set => Set(ref clippingArg, value); }
        private ClippingArgBase clippingArg = new DontClipParameter();

        [Display(Name = nameof(Texts.PartParam_LayerInfo_SourseSelectArg_Parent), ResourceType = typeof(Texts))]
        [TextEditor]
        public string Point { get => point; set => Set(ref point, value); }
        private string point = string.Empty;

        [Display(Name = nameof(Texts.PartParam_LayerInfo_SourseSelectArg_PlaybackSpeed), ResourceType = typeof(Texts))]
        [FrameNumberEditor]
        [DefaultValue(0)]
        [Range(0, 99999)]
        public double PlaybackSpeed { get => playbackSpeed; set => Set(ref playbackSpeed, value); }
        private double playbackSpeed = 1.0;

        [Display(Name = nameof(Texts.PartParam_LayerInfo_SourseSelectArg_StartFrameNumber), ResourceType = typeof(Texts))]
        [TextBoxSlider("F2", "%", 0, 200)]
        [DefaultValue(100)]
        [Range(0, 99999)]
        public int StartFrameNumber { get => startFrameNumber; set => Set(ref startFrameNumber, value); }
        private int startFrameNumber = 0;

        [Display(Name = nameof(Texts.PartParam_LayerInfo_SourseSelectArg_LoopPlayback), ResourceType = typeof(Texts))]
        [ToggleSlider]
        public bool LoopPlayback { get => loopPlayback; set => Set(ref loopPlayback, value); }
        private bool loopPlayback = false;

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
                ClippingArg = clippingParameter.ClippingArg;
            }
            if (origin is IPointParameter pointParameter)
                Point = pointParameter.Point;
            if (origin is IPlaybackSpeedParameter playbackSpeedParameter)
                PlaybackSpeed = playbackSpeedParameter.PlaybackSpeed;
            if (origin is IStartFrameNumberParameter startFrameNumberParameter)
                StartFrameNumber = startFrameNumberParameter.StartFrameNumber;
            if (origin is ILoopPlaybackParameter loopPlaybackParameter)
                LoopPlayback = loopPlaybackParameter.LoopPlayback;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new ParentSharedData(this));
            store.Save(new SceneIdSharedData(this));
            store.Save(new ClipSharedData(this));
            store.Save(new PointSharedData(this));
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
            if (store.Load<PointSharedData>() is PointSharedData pointSharedData)
                pointSharedData.CopyTo(this);
            if (store.Load<PlaybackSpeedSharedData>() is PlaybackSpeedSharedData playbackSpeedSharedData)
                playbackSpeedSharedData.CopyTo(this);
            if (store.Load<StartFrameNumberSharedData>() is StartFrameNumberSharedData startFrameNumberSharedData)
                startFrameNumberSharedData.CopyTo(this);
            if (store.Load<LoopPlaybackSharedData>() is LoopPlaybackSharedData loopPlaybackSharedData)
                loopPlaybackSharedData.CopyTo(this);
        }
    }
}
