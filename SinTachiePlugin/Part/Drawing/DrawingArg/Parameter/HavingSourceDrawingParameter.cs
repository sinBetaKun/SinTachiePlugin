using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.Drawing.DrawingArg.Argument.ModeMaster;
using SinTachiePlugin.Part.Drawing.DrawingArg.Argument.SubArg;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument.Parameter;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.Parameter
{
    internal class HavingSourceDrawingParameter : DrawingArgBase, IModeMasterParameter, ISubArgParameter
    {
        [Display(Name = nameof(TextResource.PartParam_Drawing_Mode_Master), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public DrawingValueModeMaster ModeMaster { get => modeMaster; set => Set(ref modeMaster, value); }
        private DrawingValueModeMaster modeMaster = DrawingValueModeMaster.Override;

        [Display(AutoGenerateField = true)]
        public DrawingSubArgBase SubArg { get => subArg; set => Set(ref subArg, value); }
        private DrawingSubArgBase subArg = new MasterModeParameter();

        public HavingSourceDrawingParameter()
        {
        }

        public HavingSourceDrawingParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override ValueTask EndEditAsync()
        {
            SubArg = ModeMaster.Convert(SubArg);
            return base.EndEditAsync();
        }

        public override void CopyFrom(DrawingArgBase? origin)
        {
            if (origin is IModeMasterParameter modeMasterParameter)
                ModeMaster = modeMasterParameter.ModeMaster;
            if (origin is ISubArgParameter subArgParameter)
                SubArg = subArgParameter.SubArg.GetClone();
        }

        public override DrawingArgBase GetClone()
        {
            HavingSourceDrawingParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [SubArg];

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
