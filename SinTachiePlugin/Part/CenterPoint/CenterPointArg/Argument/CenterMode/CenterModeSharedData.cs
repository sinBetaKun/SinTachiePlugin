using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.Argument.CenterMode
{
    internal class CenterModeSharedData
    {
        public CenterPointMode CenterMode { get; set; } = CenterPointMode.DontSet;

        public CenterModeSharedData()
        {
        }

        public CenterModeSharedData(ICenterModeParameter parameter)
        {
            CenterMode = parameter.CenterMode;
        }

        public void CopyTo(ICenterModeParameter parameter)
        {
            parameter.CenterMode = CenterMode;
        }
    }
}
