namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg
{
    internal record PartAnimationResultB(bool IsIndex, string Text, int Index)
    {
        public static PartAnimationResultB FromText(string text) => new(false, text, 0);
        public static PartAnimationResultB FromIndex(int index) => new(true, string.Empty, index);
    }
}
