using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.Argment.DefineMode;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.Argment.SubArg;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgment;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgment.Parameter;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.Parameter
{
    internal class HavingSourceCustomPointParameter : CustomPointArgBase, IDefineModeParameter, ISubArgParameter
    {
        [EnumComboBox]
        public CustomPointDefineMode DefineMode { get => _defineMode; set => Set(ref _defineMode, value); }
        private CustomPointDefineMode _defineMode = CustomPointDefineMode.DontMake;

        [Display(AutoGenerateField = true)]
        public CustomPointSubArgBase SubArg { get => _subArg; set => Set(ref _subArg, value); }
        private CustomPointSubArgBase _subArg = new NothingParameter();

        public HavingSourceCustomPointParameter()
        {
        }

        public HavingSourceCustomPointParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override ValueTask EndEditAsync()
        {
            SubArg = DefineMode.Convert(SubArg);
            return base.EndEditAsync();
        }

        public override CustomPointArgBase GetClone()
        {
            HavingSourceCustomPointParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        public override void CopyFrom(CustomPointArgBase? origin)
        {
            if (origin is IDefineModeParameter defineModeParameter)
                DefineMode = defineModeParameter.DefineMode;
            if (origin is ISubArgParameter subArgParameter)
                SubArg = subArgParameter.SubArg.GetClone();
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [SubArg];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new DefineModeSharedData(this));
            store.Save(new SubArgSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<DefineModeSharedData>() is DefineModeSharedData defineModeSharedData)
                defineModeSharedData.CopyTo(this);
            if (store.Load<SubArgSharedData>() is SubArgSharedData subArgSharedData)
                subArgSharedData.CopyTo(this);
        }
    }
}