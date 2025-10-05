namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.PsdFilePath
{
    internal class PsdFilePathSharedData
    {
        public string PsdFilePath { get; set; } = string.Empty;

        public PsdFilePathSharedData()
        {
        }

        public PsdFilePathSharedData(IPsdFilePathParameter parameter)
        {
            PsdFilePath = parameter.PsdFilePath;
        }

        public void CopyTo(IPsdFilePathParameter parameter)
        {
            parameter.PsdFilePath = PsdFilePath;
        }
    }
}
