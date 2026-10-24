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

            foreach (PartValueAndFL pvfl_0 in pvfls)
            {
                ControlledParametersOfPart cpp_0 = pvfl_0.PartValue.ControlledParameters;

                if (cpp_0.SourceSelectArg is IClipParameter clipParameter)
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

                if (cpp_0.SourceSelectArg is IParentParameter parentParameter)
                    if (!string.IsNullOrEmpty(parentParameter.Parent))
                        Parent = parentParameter.Parent;

                if (cpp_0.SourceSelectArg is IInsertParameter insertParameter)
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
            // PartNetwork は親子関係の変化
            bool b_4n_0 = pg.Parent != Parent;

            // Value でクリッピング先の変化を通知
            bool b_4v_0 = (b_4n_0 && pg.ClippingMode == ClippingMode.ClipToParent)
                || pg.ClippingMode != ClippingMode
                || pg.TagToClipTo != TagToClipTo;

            // Orderは描画順序の変化
            bool b_4o_0 = (b_4n_0 && pg.InsertDefineMode == PartInsertDefineMode.ToParentAndPosition)
                || pg.InsertDefineMode != InsertDefineMode
                || pg.TagToInsert != TagToInsert
                || pg.InsertPosition != InsertPosition;

            return new(Order: b_4o_0, PartNetwork: b_4n_0, Value: b_4v_0);
        }
    }
}
