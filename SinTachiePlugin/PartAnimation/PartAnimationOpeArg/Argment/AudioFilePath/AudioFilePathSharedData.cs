namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.AudioFilePath
{
    internal class AudioFilePathSharedData
    {
        public string AudioFilePath { get; set; } = string.Empty;

        public AudioFilePathSharedData()
        {
        }

        public AudioFilePathSharedData(IAudioFilePathParameter parameter)
        {
            AudioFilePath = parameter.AudioFilePath;
        }

        public void CopyTo(IAudioFilePathParameter parameter)
        {
            parameter.AudioFilePath = AudioFilePath;
        }
    }
}
