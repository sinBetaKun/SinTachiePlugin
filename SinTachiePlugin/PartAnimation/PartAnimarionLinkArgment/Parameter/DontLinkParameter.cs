using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Parameter
{
    internal class DontLinkParameter : PartAnimationLinkArgBase
    {
        public DontLinkParameter()
        {
        }

        public DontLinkParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyTo(PartAnimationLinkArgBase? origin)
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
