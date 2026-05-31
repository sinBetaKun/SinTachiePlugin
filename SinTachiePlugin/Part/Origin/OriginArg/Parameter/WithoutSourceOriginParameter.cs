using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Origin.OriginArg.Parameter
{
    internal class WithoutSourceOriginParameter : OriginArgBase
    {
        public WithoutSourceOriginParameter()
        {
        }

        public WithoutSourceOriginParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(OriginArgBase? origin)
        {
        }

        public override OriginArgBase GetClone()
        {
            return new WithoutSourceOriginParameter();
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
