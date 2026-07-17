namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg
{
    internal record PartAnimationResultA(bool IsVolume, string Text, double Volume)
    {
        public static PartAnimationResultA FromText(string text) => new(false, text, 0);
        public static PartAnimationResultA FromVolume(double volume) => new(true, string.Empty, volume);
    }
}
