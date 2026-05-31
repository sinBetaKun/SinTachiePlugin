using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.ValueDependent.ValueDependentArg.SubArgument.Parameter;

namespace SinTachiePlugin.Part.ValueDependent.ValueDependentArg.SubArgument
{
    internal static class ValueDependentModeMasterEx
    {
        public static ValueDependentSubArgBase Convert(this ValueDependentModeMaster mode, ValueDependentSubArgBase current)
        {
            var store = current.GetSharedData();
            ValueDependentSubArgBase param = mode switch
            {
                ValueDependentModeMaster.On => new MasterModeParameter(store),
                ValueDependentModeMaster.Off => new MasterModeParameter(store),
                ValueDependentModeMaster.DontOverride => new MasterModeParameter(store),
                ValueDependentModeMaster.Custom => new CustomModeParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
