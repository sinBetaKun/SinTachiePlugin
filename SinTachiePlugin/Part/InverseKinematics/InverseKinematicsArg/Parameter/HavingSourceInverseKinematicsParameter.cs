using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.Argument.IkMode;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.Argument.SubArg;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Parameter;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.Parameter
{
    internal class HavingSourceInverseKinematicsParameter : InverseKinematicsArgBase, IIkModeParameter, ISubArgParameter
    {
        [Display(Name = nameof(TextResource.InverseKinematics_IkMode), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public IKMode IkMode { get => _ikMode; set => Set(ref _ikMode, value); }
        private IKMode _ikMode = IKMode.None;

        [Display(AutoGenerateField = true)]
        public InverseKinematicsSubArgBase SubArg { get => _subArg; set => Set(ref _subArg, value); }
        private InverseKinematicsSubArgBase _subArg = new NoneIKParameter();

        public HavingSourceInverseKinematicsParameter()
        {
        }

        public HavingSourceInverseKinematicsParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override ValueTask EndEditAsync()
        {
            SubArg = IkMode.Convert(SubArg);
            return base.EndEditAsync();
        }


        public override void CopyFrom(InverseKinematicsArgBase? origin)
        {
            if (origin is IIkModeParameter ikModeParameter)
                IkMode = ikModeParameter.IkMode;
            if (origin is ISubArgParameter subArgParameter)
                SubArg = subArgParameter.SubArg.GetClone();
        }

        public override InverseKinematicsArgBase GetClone()
        {
            HavingSourceInverseKinematicsParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [SubArg];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new IkModeSharedData(this));
            store.Save(new SubArgSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<IkModeSharedData>() is IkModeSharedData ikModeSharedData)
                IkMode = ikModeSharedData.IkMode;
            if (store.Load<SubArgSharedData>() is SubArgSharedData subArgSharedData)
                SubArg = subArgSharedData.SubArg.GetClone();
        }
    }
}
