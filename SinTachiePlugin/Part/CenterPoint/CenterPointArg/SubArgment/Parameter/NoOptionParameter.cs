using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment.Parameter
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

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
        }
    }
}
