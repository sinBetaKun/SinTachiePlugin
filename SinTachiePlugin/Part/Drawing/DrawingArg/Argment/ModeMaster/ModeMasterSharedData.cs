using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.Argment.ModeMaster
{
    internal class ModeMasterSharedData
    {
        public DrawingValueModeMaster ModeMaster { get; set; } = DrawingValueModeMaster.Override;

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
