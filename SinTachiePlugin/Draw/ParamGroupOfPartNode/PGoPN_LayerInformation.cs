using SinTachiePlugin.Enums;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.ClippingArg.Argument.TagToClipTo;
using SinTachiePlugin.Part.LayerInformation.InsertArg.Argument.InsertPosition;
using SinTachiePlugin.Part.LayerInformation.InsertArg.Argument.TagToInsert;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Clip;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Insert;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Parent;

namespace SinTachiePlugin.Draw.ParamGroupOfPartNode
{
    internal class PGoPN_LayerInformation
    {
        public string Parent { get; private set; } = string.Empty;
        public ClippingMode ClippingMode { get; private set; }
        public string TagToClipTo { get; private set; } = string.Empty;
        public PartInsertDefineMode InsertDefineMode { get; private set; }
        public string TagToInsert { get; private set; } = string.Empty;
        public PartInsertPosition InsertPosition { get; private set; }

        public void Reset()
        {
            Parent = string.Empty;
            ClippingMode = ClippingMode.DontClip;
            TagToClipTo = string.Empty;
            InsertDefineMode = PartInsertDefineMode.DontInsert;
            TagToInsert = string.Empty;
            InsertPosition = PartInsertPosition.Back;
        }

        public void Update(List<PartValueAndFL> pvfls)
        {
            Parent = string.Empty;
            ClippingMode = ClippingMode.DontClip;
            TagToClipTo = string.Empty;
            InsertDefineMode = PartInsertDefineMode.DontInsert;
            TagToInsert = string.Empty;
            InsertPosition = PartInsertPosition.Back;

            foreach (PartValueAndFL pvfl in pvfls)
            {
                ControlledParametersOfPart cpp = pvfl.PartValue.ControlledParameters;

                if (cpp.SourceSelectArg is IClipParameter clipParameter)
                {
                    if (clipParameter.ClippingMode != ClippingMode.DontOverride)
                    {
                        ClippingMode = clipParameter.ClippingMode;

                        if (clipParameter.ClippingArg is ITagToClipToParameter partToClipToParameter)
                        {
                            TagToClipTo = partToClipToParameter.TagToClipTo;
                        }
                        else
                        {
                            TagToClipTo = string.Empty;
                        }
                    }
                }

                if (cpp.SourceSelectArg is IParentParameter parentParameter)
                    if (string.IsNullOrEmpty(parentParameter.Parent))
                        Parent = parentParameter.Parent;

                if (cpp.SourceSelectArg is IInsertParameter insertParameter)
                {
                    if (insertParameter.InsertDefineMode != PartInsertDefineMode.DontOverride)
                    {
                        InsertDefineMode = insertParameter.InsertDefineMode;

                        if (insertParameter.InsertArg is ITagToInsertParameter tagToInsertParameter)
                        {
                            TagToInsert = tagToInsertParameter.TagToInsert;
                        }
                        else
                        {
                            TagToInsert = string.Empty;
                        }

                        if (insertParameter.InsertArg is IInsertPositionParameter insertPositionParameter)
                        {
                            InsertPosition = insertPositionParameter.InsertPosition;
                        }
                    }
                }
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
            bool networkChanged = pg.Parent != Parent;
            bool orderChanged = (networkChanged && pg.InsertDefineMode == PartInsertDefineMode.ToParentAndPosition)
                || pg.InsertDefineMode != InsertDefineMode
                || pg.TagToInsert != TagToInsert
                || pg.InsertPosition != InsertPosition;
            bool vChanged = networkChanged || orderChanged
                || pg.ClippingMode != ClippingMode
                || pg.TagToClipTo != TagToClipTo;

            return new(Order: orderChanged, PartNetwork: networkChanged, Value: vChanged);
        }
    }
}
