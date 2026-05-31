using System.Collections.Immutable;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgument.Argument.Points
{
    internal class PointsSharedData
    {
        public ImmutableList<CustomNamePoint> Points { get; set; } = [];

        public PointsSharedData()
        {
        }

        public PointsSharedData(IPointsParameter parameter)
        {
            Points = [.. parameter.Points.Select(p => new CustomNamePoint(p))];
        }

        public void CopyTo(IPointsParameter parameter)
        {
            parameter.Points = [.. Points.Select(p => new CustomNamePoint(p))];
        }
    }
}
