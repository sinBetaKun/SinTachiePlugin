using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.Argument.CenterMode;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.Argument.SubArg;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument.Parameter;
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
        public CenterPointMode CenterMode { get => _centerMode; set => Set(ref _centerMode, value); }
        private CenterPointMode _centerMode = CenterPointMode.DontSet;

        [Display(AutoGenerateField = true)]
        public CenterPointSubArgBase SubArg { get => _subArg; set => Set(ref _subArg, value); }
        private CenterPointSubArgBase _subArg = new NoOptionParameter();

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
                CenterMode = centerModeParameter.CenterMode;
            if (origin is ISubArgParameter subArgParameter)
                SubArg = subArgParameter.SubArg.GetClone();
        }

        public override CenterPointArgBase GetClone()
        {
            HavingSourceCenterPointParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [SubArg];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new CenterModeSharedData(this));
            store.Save(new SubArgSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<CenterModeSharedData>() is CenterModeSharedData centerModeSharedData)
                centerModeSharedData.CopyTo(this);
            if (store.Load<SubArgSharedData>() is SubArgSharedData subArgSharedData)
                subArgSharedData.CopyTo(this);
        }
    }
}
