using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.SecundaryInterval
{
    internal class SecondaryIntervalSharedData
    {
        public Animation SecondaryInterval { get; } = new(0, 0, 9999);

        public SecondaryIntervalSharedData()
        {
        }

        public SecondaryIntervalSharedData(ISecondaryIntervalParameter parameter)
        {
            SecondaryInterval.CopyFrom(parameter.SecondaryInterval);
        }

        public void CopyTo(ISecondaryIntervalParameter parameter)
        {
            parameter.SecondaryInterval.CopyFrom(SecondaryInterval);
        }
    }
}
