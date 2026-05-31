namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.VideoFilePath
{
    internal class VideoFilePathSharedData
    {
        public string VideoFilePath { get; set; } = string.Empty;

        public VideoFilePathSharedData()
        {
        }

        public VideoFilePathSharedData(IVideoFilePathParameter parameter)
        {
            VideoFilePath = parameter.VideoFilePath;
        }

        public void CopyTo(IVideoFilePathParameter parameter)
        {
            parameter.VideoFilePath = VideoFilePath;
        }
    }
}
