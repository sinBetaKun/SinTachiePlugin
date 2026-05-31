using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Draw
{
    internal class PartAnimationArg
    {
        public string AnimationTag = string.Empty;
        public int Index = 0;
        public double Volume = 0;
        public string Text = string.Empty;
        public bool IsVolume = true;
        public PartAnimationNormalizationMode NormalizationMode = PartAnimationNormalizationMode.Limit;
        public PartAnimationLinkOpeMode LinkOpeMode = PartAnimationLinkOpeMode.DontLink;
        public string TargetPartTag = string.Empty;
        public string TargetAnimationTag = string.Empty;
    }
}
