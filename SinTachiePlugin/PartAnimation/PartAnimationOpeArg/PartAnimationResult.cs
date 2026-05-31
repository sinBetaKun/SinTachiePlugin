namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg
{
    internal record PartAnimationResult(bool IsVolume, string Text, double Volume)
    {
        public static PartAnimationResult FromText(string text) => new(false, text, 0);
        public static PartAnimationResult FromVolume(double volume) => new(true, string.Empty, volume);
    }
}
