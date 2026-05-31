using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.PartEffect.PartEffectArg.Argument.OverrideMode
{
    internal class PEOverrideModeSharedData
    {
        public PartEffectOverrideMode PEOverrideMode { get; set; } = PartEffectOverrideMode.Add;

        public PEOverrideModeSharedData()
        {
        }

        public PEOverrideModeSharedData(IPEOverrideModeParameter parameter)
        {
            PEOverrideMode = parameter.PEOverrideMode;
        }

        public void CopyTo(IPEOverrideModeParameter parameter)
        {
            parameter.PEOverrideMode = PEOverrideMode;
        }
    }
}
