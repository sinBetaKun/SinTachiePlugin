using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.PartEffect.PartEffectArg.Parameter
{
    internal class WithoutSourcePartEffectParameter : PartEffectArgBase
    {
        public WithoutSourcePartEffectParameter()
        {
        }

        public WithoutSourcePartEffectParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(PartEffectArgBase? origin)
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
