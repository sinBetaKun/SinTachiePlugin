namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.IsCompressFrame
{
    internal class IsCompressFrameShaerdData
    {
        public bool IsCompressFrame { get; set; } = false;

        public IsCompressFrameShaerdData()
        {
        }

        public IsCompressFrameShaerdData(IIsCompressFrameParameter parameter)
        {
            IsCompressFrame = parameter.IsCompressFrame;
        }

        public void CopyTo(IIsCompressFrameParameter parameter)
        {
            parameter.IsCompressFrame = IsCompressFrame;
        }
    }
}
