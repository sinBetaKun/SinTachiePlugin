using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.ValueDependent.ValueDependentArg.Parameter
{
    internal class WithoutSourceValueDependentParameter : ValueDependentArgBase
    {
        public WithoutSourceValueDependentParameter()
        {
        }

        public WithoutSourceValueDependentParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(ValueDependentArgBase? origin)
        {
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
