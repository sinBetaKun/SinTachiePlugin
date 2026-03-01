using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.Argment.DefineMode
{
    internal class DefineModeSharedData
    {
        public CustomPointDefineMode DefineMode { get; set; } = CustomPointDefineMode.DontMake;

        public DefineModeSharedData()
        {
        }

        public DefineModeSharedData(IDefineModeParameter parameter)
        {
            DefineMode = parameter.DefineMode;
        }

        public void CopyTo(IDefineModeParameter parameter)
        {
            parameter.DefineMode = DefineMode;
        }
    }
}
