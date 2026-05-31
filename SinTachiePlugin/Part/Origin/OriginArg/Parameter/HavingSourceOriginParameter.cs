using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.Origin.OriginArg.Argument.OriginMode;
using SinTachiePlugin.Part.Origin.OriginArg.Argument.SubArg;
using SinTachiePlugin.Part.Origin.OriginArg.SubArgument;
using SinTachiePlugin.Part.Origin.OriginArg.SubArgument.Parameter;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Origin.OriginArg.Parameter
{
    internal class HavingSourceOriginParameter : OriginArgBase, IOriginModeParameter, ISubArgParameter
    {
        [Display(Name = nameof(TextResource.PartParam_Origin_OriginMode), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public OriginDefineMode OriginMode { get => _originDefineMode; set => Set(ref _originDefineMode, value); }
        private OriginDefineMode _originDefineMode = OriginDefineMode.CenterOfParentSource;

        [Display(AutoGenerateField = true)]
        public OriginSubArgBase SubArg { get => _subArg; set => Set(ref _subArg, value); }
        private OriginSubArgBase _subArg = new OriginNoneOptionParameter();

        public HavingSourceOriginParameter()
        {
        }

        public HavingSourceOriginParameter(SharedDataStore? store = null) : base(store)
        {
        }
        public override ValueTask EndEditAsync()
        {
            SubArg = OriginMode.Convert(SubArg);
            return base.EndEditAsync();
        }

        public override void CopyFrom(OriginArgBase? origin)
        {
            if (origin is IOriginModeParameter originModeParameter)
                OriginMode = originModeParameter.OriginMode;
            if (origin is ISubArgParameter subArgParameter)
                SubArg = subArgParameter.SubArg.GetClone();
        }

        public override OriginArgBase GetClone()
        {
            HavingSourceOriginParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [SubArg];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new OriginModeSharedData(this));
            store.Save(new SubArgSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<OriginModeSharedData>() is OriginModeSharedData originModeSharedData)
                OriginMode = originModeSharedData.OriginMode;
            if (store.Load<SubArgSharedData>() is SubArgSharedData subArgSharedData)
                SubArg = subArgSharedData.SubArg.GetClone();
        }
    }
}
