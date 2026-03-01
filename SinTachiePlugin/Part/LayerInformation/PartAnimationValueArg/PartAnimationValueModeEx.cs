using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Parameter;

namespace SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg
{
    internal static class PartAnimationValueModeEx
    {
        public static PartAnimationValueArgBase Convert(this PartAnimationValueMode mode, PartAnimationValueArgBase current)
        {
            var store = current.GetSharedData();
            PartAnimationValueArgBase param = mode switch
            {
                PartAnimationValueMode.None => new NoneValueParameter(store),
                PartAnimationValueMode.Single => new SingleValueParameter(store),
                PartAnimationValueMode.Multi => new MultiValuesParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
