namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.StartFrameNumber
{
    internal class StartFrameNumberSharedData
    {
        public int StartFrameNumber { get; set; } = 0;

        public StartFrameNumberSharedData()
        {
        }

        public StartFrameNumberSharedData(IStartFrameNumberParameter parameter)
        {
            StartFrameNumber = parameter.StartFrameNumber;
        }

        public void CopyTo(IStartFrameNumberParameter parameter)
        {
            parameter.StartFrameNumber = StartFrameNumber;
        }
    }
}
