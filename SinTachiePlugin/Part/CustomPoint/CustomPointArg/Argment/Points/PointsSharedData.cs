using System.Collections.Immutable;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.Argment.Points
{
    internal class PointsSharedData
    {
        public ImmutableList<CustomNamePoint> Points { get; set; } = [];

        public PointsSharedData()
        {
        }

        public PointsSharedData(IPointsParameter parameter)
        {
            Points = [.. parameter.Points];
        }

        public void CopyTo(IPointsParameter parameter)
        {
            parameter.Points = [.. Points];
        }
    }
}
