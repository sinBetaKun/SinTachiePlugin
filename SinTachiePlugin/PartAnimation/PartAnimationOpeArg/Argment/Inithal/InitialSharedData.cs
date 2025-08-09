using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Inithal
{
    internal class InitialSharedData
    {
        public Animation Initial { get; } = new Animation(0, -10000, 10000);

        public InitialSharedData()
        {
        }

        public InitialSharedData(IInitialParameter parameter)
        {
            Initial.CopyFrom(parameter.Initial);
        }

        public void CopyTo(IInitialParameter parameter)
        {
            parameter.Initial.CopyFrom(Initial);
        }
    }
}
