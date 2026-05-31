using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.ValueDependent.ValueDependentArg.Argument.ModeMaster
{
    internal class ModeMasterSharedData
    {
        public ValueDependentModeMaster ModeMaster { get; set; } = ValueDependentModeMaster.On;

        public ModeMasterSharedData()
        {
        }

        public ModeMasterSharedData(IModeMasterParameter parameter)
        {
            ModeMaster = parameter.ModeMaster;
        }

        public void CopyTo(IModeMasterParameter parameter)
        {
            parameter.ModeMaster = ModeMaster;
        }
    }
}
