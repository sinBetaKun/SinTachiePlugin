namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.AudioFilePath
{
    internal interface IAudioFilePathParameter
    {
        public string AudioFilePath { get; set; }
        public int AudioTrackIndex { get; set; }
        public int Fluency { get; set; }
    }
}
