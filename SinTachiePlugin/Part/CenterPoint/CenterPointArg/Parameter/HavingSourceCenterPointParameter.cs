using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.Argment.CenterMode;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.Argment.SubArg;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment.Parameter;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.Parameter
{
    internal class HavingSourceCenterPointParameter : CenterPointArgBase, ICenterModeParameter, ISubArgParameter
    {
        [Display(Name = nameof(TextResource.PartParam_CenterPoint_CenterMode), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public CenterPointMode CenterMode { get => centerMode; set => Set(ref centerMode, value); }
        private CenterPointMode centerMode = CenterPointMode.OfPart;

        [Display(AutoGenerateField = true)]
        public CenterPointSubArgBase SubArg { get => subArg; set => Set(ref subArg, value); }
        private CenterPointSubArgBase subArg = new NoOptionParameter();

        public HavingSourceCenterPointParameter()
        {
        }

        public HavingSourceCenterPointParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override ValueTask EndEditAsync()
        {
            SubArg = CenterMode.Convert(SubArg);
            return base.EndEditAsync();
        }

        public override void CopyFrom(CenterPointArgBase? origin)
        {
            if (origin is ICenterModeParameter centerModeParameter) 
                centerMode = centerModeParameter.CenterMode;
            if (origin is ISubArgParameter subArgParameter)
                subArg = subArgParameter.SubArg;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new CenterModeSharedData(this));
            store.Save(new SubArgSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<CenterModeSharedData>() is CenterModeSharedData centerModeSharedData)
                CenterMode = centerModeSharedData.CenterMode;
            if (store.Load<SubArgSharedData>() is SubArgSharedData subArgSharedData)
                SubArg = CenterMode.GetClone(subArgSharedData.SubArg);
        }
    }
}
