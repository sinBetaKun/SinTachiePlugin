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

        public PartAnimationArg GetClone()
            => new()
            {
                AnimationTag = AnimationTag,
                Index = Index,
                Volume = Volume,
                Text = Text,
                IsVolume = IsVolume,
                NormalizationMode = NormalizationMode,
                LinkOpeMode = LinkOpeMode,
                TargetPartTag = TargetPartTag,
                TargetAnimationTag = TargetAnimationTag,
            };

        public bool IsEqual(PartAnimationArg other)
            =>  AnimationTag == other.AnimationTag &&
                Index == other.Index &&
                Volume == other.Volume &&
                Text == other.Text &&
                IsVolume == other.IsVolume &&
                NormalizationMode == other.NormalizationMode &&
                LinkOpeMode == other.LinkOpeMode &&
                TargetPartTag == other.TargetPartTag &&
                TargetAnimationTag == other.TargetAnimationTag;
    }
}
