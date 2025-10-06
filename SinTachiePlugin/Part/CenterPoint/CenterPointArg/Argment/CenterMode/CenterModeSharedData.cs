using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.Argment.CenterMode
{
    internal class CenterModeSharedData
    {
        public CenterPointMode CenterMode { get; set; } = CenterPointMode.OfPart;

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
