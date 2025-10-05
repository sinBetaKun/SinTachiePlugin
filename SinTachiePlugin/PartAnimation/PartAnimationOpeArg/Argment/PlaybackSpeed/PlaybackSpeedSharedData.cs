namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.PlaybackSpeed
{
    internal class PlaybackSpeedSharedData
    {
        public double PlaybackSpeed { get; set; } = 100;

        public PlaybackSpeedSharedData()
        {
        }

        public PlaybackSpeedSharedData(IPlaybackSpeedParameter parameter)
        {
            PlaybackSpeed = parameter.PlaybackSpeed;
        }

        public void CopyTo(IPlaybackSpeedParameter parameter)
        {
            parameter.PlaybackSpeed = PlaybackSpeed;
        }
    }
}
