namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument.Argument.CustomPointName
{
    internal class CustomPointNameSharedData
    {
        public string CustomPointName { get; set; } = string.Empty;

        public CustomPointNameSharedData()
        {
        }

        public CustomPointNameSharedData(ICustomPointNameParameter parameter)
        {
            CustomPointName = parameter.CustomPointName;
        }

        public void CopyTo(ICustomPointNameParameter parameter)
        {
            parameter.CustomPointName = CustomPointName;
        }
    }
}
