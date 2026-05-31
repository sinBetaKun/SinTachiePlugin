using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument.Parameter;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument
{
    internal static class CenterPointModeEx
    {
        public static CenterPointSubArgBase Convert(this CenterPointMode mode, CenterPointSubArgBase current)
        {
            var store = current.GetSharedData();
            CenterPointSubArgBase param = mode switch
            {
                CenterPointMode.DontOverride => new NoOptionParameter(store),
                CenterPointMode.DontSet => new NoOptionParameter(store),
                CenterPointMode.OnlyCoordinate => new OnlyCoordinateParameter(store),
                CenterPointMode.CustomPointName => new CustomCenterPointParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
