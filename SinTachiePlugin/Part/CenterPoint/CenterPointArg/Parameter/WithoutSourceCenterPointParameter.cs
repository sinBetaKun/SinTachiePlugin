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

        public override CenterPointArgBase GetClone()
        {
            WithoutSourceCenterPointParameter clone = new();
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
