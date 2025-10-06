using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.Parameter
{
    internal class WithoutSourceCenterPointParameter : CenterPointArgBase
    {
        public WithoutSourceCenterPointParameter()
        {
        }

        public WithoutSourceCenterPointParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(CenterPointArgBase? origin)
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
