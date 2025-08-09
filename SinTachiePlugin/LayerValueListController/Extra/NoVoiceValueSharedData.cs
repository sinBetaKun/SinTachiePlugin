using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.LayerValueListController.Extra
{
    [Obsolete]
    internal class NoVoiceValueSharedData
    {
        public Animation NoVoiceValue { get; } = new Animation(0, -10000, 10000);

        public NoVoiceValueSharedData()
        {
        }

        public NoVoiceValueSharedData(INoVoiceValueParameter parameter)
        {
            NoVoiceValue.CopyFrom(parameter.NoVoiceValue);
        }

        public void CopyTo(INoVoiceValueParameter parameter)
        {
            parameter.NoVoiceValue.CopyFrom(NoVoiceValue);
        }
    }
}
