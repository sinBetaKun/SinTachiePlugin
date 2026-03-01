using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment.Parameter;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment
{
    internal static class CenterPointModeEx
    {
        public static CenterPointSubArgBase Convert(this CenterPointMode mode, CenterPointSubArgBase current)
        {
            var store = current.GetSharedData();
            CenterPointSubArgBase param = mode switch
            {
                CenterPointMode.OfPart => new OnlyCoordinateParameter(store),
                CenterPointMode.OfImage => new OnlyCoordinateParameter(store),
                CenterPointMode.CustomPointName => new CustomCenterPointParameter(store),
                CenterPointMode.DontOverride => new NoOptionParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
