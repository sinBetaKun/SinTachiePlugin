using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Draw
{
    internal class PartAnimationArg
    {
        public string AnimationTag = string.Empty;
        public int Index = 0;
        public double Value = 0;
        public PartAnimationNormalizationMode NormalizationMode = PartAnimationNormalizationMode.Limit;
        public PartAnimationLinkOpeMode LinkOpeMode = PartAnimationLinkOpeMode.DontLink;
        public string TargetPartTag = string.Empty;
        public string TargetAnimationTag = string.Empty;
    }
}
