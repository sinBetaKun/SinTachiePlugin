namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment.Armgent.CustomPointName
{
    internal class CustomPointNameSharedData
    {
        public string CustomPoint { get; set; } = string.Empty;

        public CustomPointNameSharedData()
        {
        }

        public CustomPointNameSharedData(ICustomPointNameParameter parameter)
        {
            CustomPoint = parameter.CustomPoint;
        }

        public void CopyTo(ICustomPointNameParameter parameter)
        {
            parameter.CustomPoint = CustomPoint;
        }
    }
}
