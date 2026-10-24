using SinTachiePlugin.Draw.PointParam;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.Parameter;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgument;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgument.Parameter;

namespace SinTachiePlugin.Draw.ParamGroupOfPartNode
{
    internal class PGoPN_CustomPoint
    {
        public List<CustomNamePointParam> CustomNamePoints { get; private set; } = [];

        public void Reset()
        {
            CustomNamePoints.Clear();
        }

        public void Update(List<PartValueAndFL> pvfls)
        {
            CustomNamePoints.Clear();

            foreach (PartValueAndFL pvfl in pvfls)
            {
                PartValue pv = pvfl.PartValue;

                if (pv.ControlledParameters.CustomPointArg is HavingSourceCustomPointParameter hscpp)
                {
                    if (hscpp.SubArg is CustomNamePointsParameter cnpp)
                    {
                        foreach (CustomNamePoint cnp_0 in cnpp.Points)
                        {
                            if (CustomNamePoints.FirstOrDefault(p => p.Name == cnp_0.Name) is CustomNamePointParam cnp_1)
                                CustomNamePoints.Remove(cnp_1);

                            CustomNamePoints.Add(new(cnp_0));
                        }
                    }
                }
            }
        }

        public void CopyTo(PGoPN_CustomPoint pg)
        {
            pg.CustomNamePoints = [.. CustomNamePoints.Select(p => new CustomNamePointParam(p))];
        }

        public PartNodeComparateResult Comparate(PGoPN_CustomPoint pg)
        {
            if (pg.CustomNamePoints.Count != CustomNamePoints.Count)
                return new(Value: true);

            for (int i_0 = 0; i_0 < pg.CustomNamePoints.Count; i_0++)
                if (CustomNamePoints[i_0].Name != pg.CustomNamePoints[i_0].Name
                    || CustomNamePoints[i_0].X != pg.CustomNamePoints[i_0].X
                    || CustomNamePoints[i_0].Y != pg.CustomNamePoints[i_0].Y)
                    return new(Value: true);

            return new();
        }
    }
}
