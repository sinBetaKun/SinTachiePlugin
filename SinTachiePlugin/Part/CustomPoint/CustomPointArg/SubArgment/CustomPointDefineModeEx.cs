using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgment.Parameter;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgment
{
    internal static class CustomPointDefineModeEx
    {
        public static CustomPointSubArgBase Convert(this CustomPointDefineMode mode, CustomPointSubArgBase current)
        {
            SharedDataStore store = current.GetSharedData();
            CustomPointSubArgBase param = mode switch
            {
                CustomPointDefineMode.DontMake => new NothingParameter(store),
                CustomPointDefineMode.DoMake => new CustomNamePointsParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }

        public static CustomPointSubArgBase GetClone(this CustomPointDefineMode mode, CustomPointSubArgBase current)
        {
            SharedDataStore store = current.GetSharedData();
            CustomPointSubArgBase param = mode switch
            {
                CustomPointDefineMode.DontMake => new NothingParameter(store),
                CustomPointDefineMode.DoMake => new CustomNamePointsParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };

            return param;
        }
    }
}
