using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument.Parameter
{
    internal class NoOptionParameter : CenterPointSubArgBase
    {
        public NoOptionParameter()
        {
        }

        public NoOptionParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(CenterPointSubArgBase? origin)
        {
        }

        public override CenterPointSubArgBase GetClone()
        {
            return new NoOptionParameter();
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
