using System.Collections.Immutable;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.Argment.Points
{
    internal interface IPointsParameter
    {
        public ImmutableList<CustomNamePoint> Points { get; set; }
    }
}
