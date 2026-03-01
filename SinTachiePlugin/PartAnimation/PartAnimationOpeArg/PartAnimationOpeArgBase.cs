using SinTachiePlugin.Draw;
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

        public abstract double GetValue(FrameAndLength fl, int fps, double voiceVolume);

        public abstract void CopyFrom(PartAnimationOpeArgBase? origin);

        public abstract PartAnimationOpeArgBase GetClone();
    }
}
