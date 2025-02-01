using SinTachiePlugin.LayerValueListController.Extra;
using SinTachiePlugin.LayerValueListController.Extra.Parameter;
using SinTachiePlugin.Parts.LayerValueListController;

namespace SinTachiePlugin.LayerValueListController
{
    internal static class LayerAnimationModeEx
    {
        public static LayerValueExtraBase Convert(this LayerAnimationMode mode, LayerValueExtraBase current)
        {
            var store = current.GetSharedData();
            LayerValueExtraBase param = mode switch
            {
                LayerAnimationMode.CerrarPlusAbrir => new NoExtra(store),
                LayerAnimationMode.CerrarTimesAbrir => new NoExtra(store),
                LayerAnimationMode.Sin => new NoExtra(store),
                LayerAnimationMode.VoiceVolume => new NoExtra(store),
                LayerAnimationMode.PeriodicShuttle => new PeriodicParameter(store),
                LayerAnimationMode.PeriodicLoop => new PeriodicParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };
            if (param.GetType() != current.GetType())
                return param;
            return current;
        }
    }
}
