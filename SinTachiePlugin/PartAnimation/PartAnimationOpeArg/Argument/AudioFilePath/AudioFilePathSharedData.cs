namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.AudioFilePath
{
    internal class AudioFilePathSharedData
    {
        public string AudioFilePath { get; set; } = string.Empty;
        public int AudioTrackIndex { get; set; } = 0;
        public int Fluency { get; set; } = 4;

        public AudioFilePathSharedData()
        {
        }

        public AudioFilePathSharedData(IAudioFilePathParameter parameter)
        {
            AudioFilePath = parameter.AudioFilePath;
            AudioTrackIndex = parameter.AudioTrackIndex;
            Fluency = parameter.Fluency;
        }

        public void CopyTo(IAudioFilePathParameter parameter)
        {
            parameter.AudioFilePath = AudioFilePath;
            parameter.AudioTrackIndex = AudioTrackIndex;
            parameter.Fluency = Fluency;
        }
    }
}
