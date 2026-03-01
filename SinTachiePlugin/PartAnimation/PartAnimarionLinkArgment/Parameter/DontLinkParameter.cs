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

        public override void CopyFrom(PartAnimationLinkArgBase? origin)
        {
        }

        public override PartAnimationLinkArgBase GetClone()
        {
            DontLinkParameter clone = new();
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
