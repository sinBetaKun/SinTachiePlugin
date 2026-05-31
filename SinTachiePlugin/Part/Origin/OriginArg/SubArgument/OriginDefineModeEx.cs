using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.Origin.OriginArg.SubArgument.Parameter;

namespace SinTachiePlugin.Part.Origin.OriginArg.SubArgument
{
    internal static class OriginDefineModeEx
    {
        public static OriginSubArgBase Convert(this OriginDefineMode mode, OriginSubArgBase current)
        {
            var store = current.GetSharedData();
            OriginSubArgBase param = mode switch
            {
                OriginDefineMode.DontOverride => new OriginNoneOptionParameter(store),
                OriginDefineMode.CenterOfParent => new OriginNoneOptionParameter(store),
                OriginDefineMode.CenterOfParentSource => new OriginNoneOptionParameter(store),
                OriginDefineMode.CustomPointOfParent => new OriginWithPointNameParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
