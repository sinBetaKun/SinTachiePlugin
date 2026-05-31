using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.Origin.OriginArg.Argument.OriginMode
{
    internal class OriginModeSharedData
    {
        public OriginDefineMode OriginMode { get; set; } = OriginDefineMode.CenterOfParentSource;

        public OriginModeSharedData()
        {
        }

        public OriginModeSharedData(IOriginModeParameter parameter)
        {
            OriginMode = parameter.OriginMode;
        }

        public void CopyTo(IOriginModeParameter parameter)
        {
            parameter.OriginMode = OriginMode;
        }
    }
}
