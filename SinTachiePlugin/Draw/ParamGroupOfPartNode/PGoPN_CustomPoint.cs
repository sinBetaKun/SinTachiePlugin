using SinTachiePlugin.Part;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.Parameter;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgument;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgument.Parameter;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Draw.ParamGroupOfPartNode
{
    internal class PGoPN_CustomPoint
    {
        public List<CustomNamePoint> CustomNamePoints { get; private set; } = [];

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
                        foreach (CustomNamePoint cnp1 in cnpp.Points)
                        {
                            if (CustomNamePoints.FirstOrDefault(p => p.Name == cnp1.Name) is CustomNamePoint cnp2)
                            {
                                CustomNamePoints.Remove(cnp1);
                                CustomNamePoints.Add(cnp2);
                            }
                        }
                    }
                }
            }
        }

        public void CopyTo(PGoPN_CustomPoint pg)
        {
            pg.CustomNamePoints = [.. CustomNamePoints.Select(p => new CustomNamePoint(p))];
        }

        public PartNodeComparateResult Comparate(PGoPN_CustomPoint pg)
        {
            if (pg.CustomNamePoints.Count != CustomNamePoints.Count)
                return new(Value: true);

            bool valueChanged = CustomNamePoints.All(p1 => pg.CustomNamePoints.Any(p2 => p1.Name == p2.Name));
            return new(Value: valueChanged);
        }
    }
}
