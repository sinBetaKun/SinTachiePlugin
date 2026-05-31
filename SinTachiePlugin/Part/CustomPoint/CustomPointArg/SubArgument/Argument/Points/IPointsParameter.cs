using System.Collections.Immutable;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgument.Argument.Points
{
    internal interface IPointsParameter
    {
        public ImmutableList<CustomNamePoint> Points { get; set; }
    }
}
