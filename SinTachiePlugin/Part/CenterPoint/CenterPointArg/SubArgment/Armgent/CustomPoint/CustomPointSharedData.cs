namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment.Armgent.CustomPoint
{
    internal class CustomPointSharedData
    {
        public string CustomPoint { get; set; } = string.Empty;

        public CustomPointSharedData()
        {
        }

        public CustomPointSharedData(ICustomPointParameter parameter)
        {
            CustomPoint = parameter.CustomPoint;
        }

        public void CopyTo(ICustomPointParameter parameter)
        {
            parameter.CustomPoint = CustomPoint;
        }
    }
}
