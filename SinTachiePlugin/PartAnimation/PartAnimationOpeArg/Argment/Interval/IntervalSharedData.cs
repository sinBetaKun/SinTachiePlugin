using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Interval
{
    internal class IntervalSharedData
    {
        public Animation Interval { get; } = new Animation(0, 0, 9999);

        public IntervalSharedData()
        {
        }

        public IntervalSharedData(IIntervalParameter parameter)
        {
            Interval.CopyFrom(parameter.Interval);
        }

        public void CopyTo(IIntervalParameter parameter)
        {
            parameter.Interval.CopyFrom(Interval);
        }
    }
}
