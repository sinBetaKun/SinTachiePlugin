using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.Argument.ModeMaster
{
    internal class ModeMasterSharedData
    {
        public DrawingValueModeMaster ModeMaster { get; set; } = DrawingValueModeMaster.Initialize;

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
