using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.Parameter;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg
{
    internal static class LayerTypeEx
    {
        public static CustomPointArgBase Convert(this LayerType type, CustomPointArgBase current)
        {
            SharedDataStore store = current.GetSharedData();
            CustomPointArgBase param = type switch
            {
                LayerType.Image => new HavingSourceCustomPointParameter(store),
                LayerType.Psd => new HavingSourceCustomPointParameter(store),
                LayerType.Video => new HavingSourceCustomPointParameter(store),
                LayerType.Scene => new HavingSourceCustomPointParameter(store),
                LayerType.Group => new WithoutSourceCustomPointParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
