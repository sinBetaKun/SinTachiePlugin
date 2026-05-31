using SinTachiePlugin.Draw;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg
{
    internal abstract class PartAnimationOpeArgBase : SharedParameterBase
    {
        public PartAnimationOpeArgBase()
        {
        }

        public PartAnimationOpeArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract PartAnimationResult GetResult(TachieSourceDescription desc);

        public abstract void CopyFrom(PartAnimationOpeArgBase? origin);

        public abstract PartAnimationOpeArgBase GetClone();
    }
}
