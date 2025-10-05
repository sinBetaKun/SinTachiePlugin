using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.ClippingArg;
using SinTachiePlugin.Part.LayerInformation.ClippingArg.Parameter;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Clip
{
    internal class ClipSharedData
    {
        public ClippingMode ClippingMode { get; set; } = ClippingMode.DontClip;

        public ClippingArgBase ClippingArg { get; set; } = new DontClipParameter();

        public ClipSharedData()
        {
        }

        public ClipSharedData(IClipParameter parameter)
        {
            ClippingMode = parameter.ClippingMode;
            ClippingArg = parameter.ClippingArg;
        }

        public void CopyTo(IClipParameter parameter)
        {
            parameter.ClippingMode = ClippingMode;
            parameter.ClippingArg = ClippingArg;
        }
    }
}
