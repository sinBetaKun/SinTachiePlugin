using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Offset
{
    internal class OffsetSharedData
    {
        public Animation Offset { get; } = new Animation(0, 0, 9999);

        public OffsetSharedData()
        {
        }

        public OffsetSharedData(IOffsetParameter parameter)
        {
            Offset.CopyFrom(parameter.Offset);
        }

        public void CopyTo(IOffsetParameter parameter)
        {
            parameter.Offset.CopyFrom(Offset);
        }
    }
}
