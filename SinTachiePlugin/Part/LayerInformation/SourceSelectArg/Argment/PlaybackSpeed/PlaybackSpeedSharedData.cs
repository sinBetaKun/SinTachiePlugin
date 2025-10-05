namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.PlaybackSpeed
{
    internal class PlaybackSpeedSharedData
    {
        public double PlaybackSpeed { get; set; } = 1.0;

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
