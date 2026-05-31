using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.ValueDependent.ValueDependentArg.Argument.ModeMaster;
using SinTachiePlugin.Part.ValueDependent.ValueDependentArg.Argument.SubArg;
using SinTachiePlugin.Part.ValueDependent.ValueDependentArg.SubArgument;
using SinTachiePlugin.Part.ValueDependent.ValueDependentArg.SubArgument.Parameter;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.ValueDependent.ValueDependentArg.Parameter
{
    internal class HavingSourceValueDependentParameter : ValueDependentArgBase, IModeMasterParameter, ISubArgParameter
    {
        [Display(Name = nameof(TextResource.PartParam_ValueDependentMode_XYZ), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public ValueDependentModeMaster ModeMaster { get => _modeMaster; set => Set(ref _modeMaster, value); }
        private ValueDependentModeMaster _modeMaster = ValueDependentModeMaster.On;

        public ValueDependentSubArgBase SubArg { get => _subArg; set => Set(ref _subArg, value);  }
        private ValueDependentSubArgBase _subArg = new MasterModeParameter();

        public HavingSourceValueDependentParameter()
        {
        }

        public HavingSourceValueDependentParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override ValueTask EndEditAsync()
        {
            SubArg = ModeMaster.Convert(SubArg);
            return base.EndEditAsync();
        }

        public override void CopyFrom(ValueDependentArgBase? origin)
        {
            if (origin is IModeMasterParameter modeMasterParameter)
                ModeMaster = modeMasterParameter.ModeMaster;
            if (origin is ISubArgParameter subArgParameter)
                SubArg = subArgParameter.SubArg.GetClone();
        }

        public override ValueDependentArgBase GetClone()
        {
            HavingSourceValueDependentParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new ModeMasterSharedData(this));
            store.Save(new SubArgSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<ModeMasterSharedData>() is ModeMasterSharedData modeMasterSharedData)
                ModeMaster = modeMasterSharedData.ModeMaster;
            if (store.Load<SubArgSharedData>() is SubArgSharedData subArgSharedData)
                SubArg = subArgSharedData.SubArg.GetClone();
        }
    }
}
