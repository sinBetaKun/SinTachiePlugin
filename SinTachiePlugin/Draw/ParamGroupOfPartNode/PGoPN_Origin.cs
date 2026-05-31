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
                PartValue pv = pvfl.PartValue;
                ControlledParametersOfPart cpp = pv.ControlledParameters;

                if (cpp.OriginArg is HavingSourceOriginParameter hsop)
                {
                    if (hsop.SubArg is OriginNoneOptionParameter)
                    {
                        if (hsop.OriginMode != OriginDefineMode.DontOverride)
                        {
                            OriginMode = hsop.OriginMode;
                            return;
                        }
                    }
                    else if (hsop.SubArg is OriginWithPointNameParameter owpnp)
                    {
                        OriginMode = hsop.OriginMode;
                        PointName = owpnp.PointName;
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
            bool valueChanged =
                pg.OriginMode != OriginMode ||
                pg.PointName != PointName;

            return new(Value: valueChanged);
        }
    }
}
