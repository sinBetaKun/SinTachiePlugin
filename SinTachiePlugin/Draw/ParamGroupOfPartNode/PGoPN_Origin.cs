using SinTachiePlugin.Enums;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.Origin.OriginArg.Parameter;
using SinTachiePlugin.Part.Origin.OriginArg.SubArgument.Parameter;

namespace SinTachiePlugin.Draw.ParamGroupOfPartNode
{
    internal class PGoPN_Origin
    {
        public OriginDefineMode OriginMode { get; private set; } = OriginDefineMode.CenterOfParentSource;
        public string PointName { get; private set; } = string.Empty;

        public void Reset()
        {
            OriginMode = OriginDefineMode.CenterOfParentSource;
            PointName = string.Empty;
        }

        public void Update(List<PartValueAndFL> pvfls)
        {
            OriginMode = OriginDefineMode.CenterOfParentSource;
            PointName = string.Empty;

            foreach (PartValueAndFL pvfl in pvfls.Reverse<PartValueAndFL>())
            {
                PartValue pv_0 = pvfl.PartValue;
                ControlledParametersOfPart cpp_0 = pv_0.ControlledParameters;

                if (cpp_0.OriginArg is HavingSourceOriginParameter hsop_0)
                {
                    if (hsop_0.SubArg is OriginNoneOptionParameter)
                    {
                        if (hsop_0.OriginMode != OriginDefineMode.DontOverride)
                        {
                            OriginMode = hsop_0.OriginMode;
                            return;
                        }
                    }
                    else if (hsop_0.SubArg is OriginWithPointNameParameter owpnp_0)
                    {
                        OriginMode = hsop_0.OriginMode;
                        PointName = owpnp_0.PointName;
                        return;
                    }
                }
            }
        }

        public void CopyTo(PGoPN_Origin pg)
        {
            pg.OriginMode = OriginMode;
            pg.PointName = PointName;
        }

        public PartNodeComparateResult Comparate(PGoPN_Origin pg)
        {
            bool b_4v =
                pg.OriginMode != OriginMode ||
                pg.PointName != PointName;

            return new(Value: b_4v);
        }
    }
}
