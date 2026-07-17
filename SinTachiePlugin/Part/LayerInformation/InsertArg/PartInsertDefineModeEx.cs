using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.InsertArg.Parameter;

namespace SinTachiePlugin.Part.LayerInformation.InsertArg
{
    internal static class PartInsertDefineModeEx
    {
        public static InsertArgBase Convert(this PartInsertDefineMode mode, InsertArgBase current)
        {
            var store = current.GetSharedData();
            InsertArgBase param = mode switch
            {
                PartInsertDefineMode.DontOverride => new DontInsertParameter(store),
                PartInsertDefineMode.DontInsert => new DontInsertParameter(store),
                PartInsertDefineMode.SelectTagAndPosition => new SelectTagAndPositionParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
