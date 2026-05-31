using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.Origin.OriginArg.Parameter;

namespace SinTachiePlugin.Part.Origin.OriginArg
{
    internal static class LayerTypeEx
    {
        public static OriginArgBase Convert(this LayerType type, OriginArgBase current)
        {
            var store = current.GetSharedData();
            OriginArgBase param = type switch
            {
                LayerType.Image => new HavingSourceOriginParameter(store),
                LayerType.Psd => new HavingSourceOriginParameter(store),
                LayerType.Video => new HavingSourceOriginParameter(store),
                LayerType.Scene => new HavingSourceOriginParameter(store),
                LayerType.Group => new WithoutSourceOriginParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
