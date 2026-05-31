using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.ValueDependent.ValueDependentArg.SubArgument.Parameter
{
    internal class MasterModeParameter : ValueDependentSubArgBase
    {
        public MasterModeParameter()
        {
        }

        public MasterModeParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(ValueDependentSubArgBase origin)
        {
        }

        public override ValueDependentSubArgBase GetClone()
        {
            MasterModeParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
        }
    }
}
