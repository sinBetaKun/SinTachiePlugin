using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.ClippingArg;
using SinTachiePlugin.Part.LayerInformation.ClippingArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Clip;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Parent;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Point;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter
{
    internal class GroupCompressFrameParameter : SourceSelectArgBase, IParentParameter, IClipParameter, IPointParameter
    {
        [Display(Name = nameof(Texts.PartParam_LayerInfo_SourseSelectArg_Parent), ResourceType = typeof(Texts))]
        [TextEditor]
        public string Parent { get => parent; set => Set(ref parent, value); }
        private string parent = string.Empty;

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

        public GroupCompressFrameParameter()
        {
        }

        public GroupCompressFrameParameter(SharedDataStore? store) : base(store)
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
            if (origin is IClipParameter clippingParameter)
            {
                ClippingMode = clippingParameter.ClippingMode;
                ClippingArg = clippingParameter.ClippingArg;
            }
            if (origin is IPointParameter pointParameter)
                Point = pointParameter.Point;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];


        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new ParentSharedData(this));
            store.Save(new ClipSharedData(this));
            store.Save(new PointSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<ParentSharedData>() is ParentSharedData parentSharedData)
                parentSharedData.CopyTo(this);
            if (store.Load<ClipSharedData>() is ClipSharedData clipSharedData)
                clipSharedData.CopyTo(this);
            if (store.Load<PointSharedData>() is PointSharedData pointSharedData)
                pointSharedData.CopyTo(this);
        }
    }
}
