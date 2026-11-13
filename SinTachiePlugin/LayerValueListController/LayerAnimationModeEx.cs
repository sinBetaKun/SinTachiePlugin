using SinTachiePlugin.LayerValueListController.Extra;
using SinTachiePlugin.LayerValueListController.Extra.Parameter;
using SinTachiePlugin.Parts.LayerValueListController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Plugin.Community.Effect.Video.ZoomPixel.Size;

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
