using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.ClippingArg;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Clip
{
    internal interface IClipParameter
    {
        public ClippingMode ClippingMode { get; set; }

        public ClippingArgBase ClippingArg { get; set; }
    }
}
