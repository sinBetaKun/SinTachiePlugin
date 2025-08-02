using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.LayerValueListController.Extra
{
    internal class StartSharedData
    {
        public Animation Start { get; } = new Animation(0, 0, 9999);

        public StartSharedData()
        {
        }

        public StartSharedData(IStartParameter parameter)
        {
            Start.CopyFrom(parameter.Start);
        }

        public void CopyTo(IStartParameter parameter)
        {
            parameter.Start.CopyFrom(Start);
        }
    }
}
