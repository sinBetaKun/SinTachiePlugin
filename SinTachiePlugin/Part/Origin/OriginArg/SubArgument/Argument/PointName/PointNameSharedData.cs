namespace SinTachiePlugin.Part.Origin.OriginArg.SubArgument.Argument.PointName
{
    internal class PointNameSharedData
    {
        public string PointName { get; set; } = string.Empty;

        public PointNameSharedData()
        {
        }

        public PointNameSharedData(IPointNameParameter parameter)
        {
            PointName = parameter.PointName;
        }

        public void CopyTo(IPointNameParameter parameter)
        {
            parameter.PointName = PointName;
        }
    }
}
