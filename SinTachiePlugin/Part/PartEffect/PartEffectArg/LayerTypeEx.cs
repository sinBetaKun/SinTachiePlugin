using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.PartEffect.PartEffectArg.Parameter;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.PartEffect.PartEffectArg
{
    internal static class LayerTypeEx
    {
        public static PartEffectArgBase Convert(this LayerType type, PartEffectArgBase current)
        {
            SharedDataStore store = current.GetSharedData();
            PartEffectArgBase param = type switch
            {
                LayerType.Image => new HavingSourcePartEffectParameter(store),
                LayerType.Psd => new HavingSourcePartEffectParameter(store),
                LayerType.Video => new HavingSourcePartEffectParameter(store),
                LayerType.Scene => new HavingSourcePartEffectParameter(store),
                LayerType.Group => new WithoutSourcePartEffectParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
