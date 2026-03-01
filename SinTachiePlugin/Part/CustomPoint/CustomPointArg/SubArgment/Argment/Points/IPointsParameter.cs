using SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgment;
using System.Collections.Immutable;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.Argment.SubArgment.Points
{
    internal interface IPointsParameter
    {
        public ImmutableList<CustomNamePoint> Points { get; set; }
    }
}
