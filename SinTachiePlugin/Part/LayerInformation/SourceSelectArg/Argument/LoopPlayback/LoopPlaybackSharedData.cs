namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.LoopPlayback
{
    internal class LoopPlaybackSharedData
    {
        public bool LoopPlayback { get; set; } = false;

        public LoopPlaybackSharedData()
        {
        }

        public LoopPlaybackSharedData(ILoopPlaybackParameter parameter)
        {
            LoopPlayback = parameter.LoopPlayback;
        }

        public void CopyTo(ILoopPlaybackParameter parameter)
        {
            parameter.LoopPlayback = LoopPlayback;
        }
    }
}
