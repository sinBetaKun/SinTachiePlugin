using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.Parameter
{
    internal class WithoutSourceCustomPointParameter : CustomPointArgBase
    {
        public WithoutSourceCustomPointParameter()
        {
        }

        public WithoutSourceCustomPointParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(CustomPointArgBase? origin)
        {
        }

        public override CustomPointArgBase GetClone()
        {
            WithoutSourceCustomPointParameter clone = new();
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
