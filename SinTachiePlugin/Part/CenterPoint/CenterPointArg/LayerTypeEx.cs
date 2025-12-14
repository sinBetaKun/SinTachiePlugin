using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.Parameter;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg
{
    internal static class LayerTypeEx
    {
        public static CenterPointArgBase Convert(this LayerType type, CenterPointArgBase current)
        {
            var store = current.GetSharedData();
            CenterPointArgBase param = type switch
            {
                LayerType.Image => new HavingSourceCenterPointParameter(store),
                LayerType.Psd => new HavingSourceCenterPointParameter(store),
                LayerType.Video => new HavingSourceCenterPointParameter(store),
                LayerType.Scene => new HavingSourceCenterPointParameter(store),
                LayerType.Group => new WithoutSourceCenterPointParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }

        public static CenterPointArgBase GetClone(this LayerType type, CenterPointArgBase origin)
        {
            var store = origin.GetSharedData();
            CenterPointArgBase param = type switch
            {
                LayerType.Image => new HavingSourceCenterPointParameter(store),
                LayerType.Psd => new HavingSourceCenterPointParameter(store),
                LayerType.Video => new HavingSourceCenterPointParameter(store),
                LayerType.Scene => new HavingSourceCenterPointParameter(store),
                LayerType.Group => new WithoutSourceCenterPointParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            return param;
        }
    }
}
