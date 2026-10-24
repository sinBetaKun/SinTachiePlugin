using SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgument;

namespace SinTachiePlugin.Draw.PointParam
{
    internal record CustomNamePointParam
    {
        public string Name;
        public double X;
        public double Y;

        public CustomNamePointParam(CustomNamePoint p)
        {
            Name = p.Name;
            X = p.X;
            Y = p.Y;
        }

        public CustomNamePointParam(CustomNamePointParam p)
        {
            Name = p.Name;
            X = p.X;
            Y = p.Y;
        }
    }
}
