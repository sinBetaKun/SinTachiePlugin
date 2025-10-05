using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.Drawing.DrawingArg.Parameter;

namespace SinTachiePlugin.Part.Drawing.DrawingArg
{
    internal static class LayerTypeEx
    {
        public static DrawingArgBase Convert(this LayerType type, DrawingArgBase current)
        {
            var store = current.GetSharedData();
            DrawingArgBase param = type switch
            {
                LayerType.Image => new HavingSourceParameter(store),
                LayerType.Psd => new HavingSourceParameter(store),
                LayerType.Video => new HavingSourceParameter(store),
                LayerType.Scene => new HavingSourceParameter(store),
                LayerType.Group => new WithoutSourceParameter(store),
                LayerType.Group_CompressFrame => new HavingSourceParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
