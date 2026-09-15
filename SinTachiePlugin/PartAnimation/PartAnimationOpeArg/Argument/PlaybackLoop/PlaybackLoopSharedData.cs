namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.PlaybackLoop
{
    internal class PlaybackLoopSharedData
    {
        public bool PlaybackLoop { get; set; } = false;

        public PlaybackLoopSharedData()
        {
        }

        public PlaybackLoopSharedData(IPlaybackLoopParameter parameter)
        {
            PlaybackLoop = parameter.PlaybackLoop;
        }

        public void CopyTo(IPlaybackLoopParameter parameter)
        {
            parameter.PlaybackLoop = PlaybackLoop;
        }
    }
}
