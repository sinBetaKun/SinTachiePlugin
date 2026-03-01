using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgment.Parameter
{
    internal class NothingParameter : CustomPointSubArgBase
    {
        public NothingParameter()
        {
        }

        public NothingParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(CustomPointSubArgBase? origin)
        {
        }

        public override CustomPointSubArgBase GetClone()
        {
            return new NothingParameter();
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
