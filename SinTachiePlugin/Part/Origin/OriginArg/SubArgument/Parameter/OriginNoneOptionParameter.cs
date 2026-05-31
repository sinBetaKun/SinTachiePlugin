using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Origin.OriginArg.SubArgument.Parameter
{
    internal class OriginNoneOptionParameter : OriginSubArgBase
    {
        public OriginNoneOptionParameter()
        {
        }

        public OriginNoneOptionParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(OriginSubArgBase? origin)
        {
        }

        public override OriginSubArgBase GetClone()
        {
            return new OriginNoneOptionParameter();
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
