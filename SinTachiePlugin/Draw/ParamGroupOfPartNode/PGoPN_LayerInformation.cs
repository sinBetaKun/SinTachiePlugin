using SinTachiePlugin.Enums;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.ClippingArg.Argument.TagToClipTo;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Clip;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Parent;

namespace SinTachiePlugin.Draw.ParamGroupOfPartNode
{
    internal class PGoPN_LayerInformation
    {
        public ClippingMode ClippingMode { get; private set; }
        public string TagToClipTo { get; private set; } = string.Empty;
        public string Parent { get; private set; } = string.Empty;

        public void Reset()
        {
            ClippingMode = ClippingMode.DontClip;
            TagToClipTo = string.Empty;
            Parent = string.Empty;
        }

        public void Update(List<PartValueAndFL> pvfls)
        {
            TagToClipTo = string.Empty;
            Parent = string.Empty;

            foreach (PartValueAndFL pvfl in pvfls)
            {
                ControlledParametersOfPart cpp = pvfl.PartValue.ControlledParameters;

                if (cpp.SourceSelectArg is IClipParameter clipParameter)
                {
                    ClippingMode = clipParameter.ClippingMode;

                    if (clipParameter.ClippingArg is ITagToClipToParameter partToClipToParameter)
                    {
                        if (!string.IsNullOrEmpty(partToClipToParameter.TagToClipTo))
                            TagToClipTo = partToClipToParameter.TagToClipTo;
                    }
                    else
                    {
                        TagToClipTo = string.Empty;
                    }
                }

                if (cpp.SourceSelectArg is IParentParameter parentParameter)
                    if (string.IsNullOrEmpty(parentParameter.Parent))
                        Parent = parentParameter.Parent;
            }
        }

        public void CopyTo(PGoPN_LayerInformation pg)
        {
            pg.ClippingMode = ClippingMode;
            pg.TagToClipTo = TagToClipTo;
            pg.Parent = Parent;
        }

        public PartNodeComparateResult Comparate(PGoPN_LayerInformation pg)
        {
            bool networkChanged = 
                pg.ClippingMode != ClippingMode ||
                pg.TagToClipTo != TagToClipTo ||
                pg.Parent != Parent;

            return new(PartNetwork: networkChanged);
        }
    }
}
