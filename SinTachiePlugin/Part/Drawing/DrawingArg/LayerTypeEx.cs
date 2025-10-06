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
                LayerType.Image => new HavingSourceDrawingParameter(store),
                LayerType.Psd => new HavingSourceDrawingParameter(store),
                LayerType.Video => new HavingSourceDrawingParameter(store),
                LayerType.Scene => new HavingSourceDrawingParameter(store),
                LayerType.Group => new WithoutSourceDrawingParameter(store),
                LayerType.Group_CompressFrame => new HavingSourceDrawingParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
