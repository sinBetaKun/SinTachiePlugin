namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgment
{
    internal class CustomNamePoint
    {
        public string Name { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public CustomNamePoint(string name, double x, double y)
        {
            Name = name;
            X = x;
            Y = y;
        }

        public CustomNamePoint(CustomNamePoint origin)
        {
            Name = origin.Name;
            X = origin.X;
            Y = origin.Y;
        }
    }
}
