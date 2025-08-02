using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.LayerValueListController.Extra
{
    internal class TransitionSharedData
    {
        public Animation Speed { get; } = new Animation(0, 0, 9999);

        public TransitionSharedData() { }

        public TransitionSharedData(ITransitionParameter parameter)
        {
            Speed.CopyFrom(parameter.Transition);
        }

        public void CopyTo(ITransitionParameter parameter)
        {
            parameter.Transition.CopyFrom(Speed);
        }
    }
}
