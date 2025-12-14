using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.ClippingArg.Parameter;

namespace SinTachiePlugin.Part.LayerInformation.ClippingArg
{
    internal static class ClippingModeEx
    {
        public static ClippingArgBase Convert(this ClippingMode mode, ClippingArgBase current)
        {
            var store = current.GetSharedData();
            ClippingArgBase param = mode switch
            {
                ClippingMode.DontClip => new DontClipParameter(store),
                ClippingMode.ClipToParent => new ClipToParentParameter(store),
                ClippingMode.ClipWithTag => new ClipWithTagParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }

        public static ClippingArgBase GetClone(this ClippingMode mode, ClippingArgBase origin)
        {
            var store = origin.GetSharedData();
            ClippingArgBase param = mode switch
            {
                ClippingMode.DontClip => new DontClipParameter(store),
                ClippingMode.ClipToParent => new ClipToParentParameter(store),
                ClippingMode.ClipWithTag => new ClipWithTagParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };

            return param;
        }
    }
}
