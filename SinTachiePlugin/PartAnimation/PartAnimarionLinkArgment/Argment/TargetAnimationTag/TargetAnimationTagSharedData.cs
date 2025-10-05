namespace SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Argment.TargetAnimationTag
{
    internal class TargetAnimationTagSharedData
    {
        public string TargetAnimationTag { get; set; } = string.Empty;

        public TargetAnimationTagSharedData()
        {
        }

        public TargetAnimationTagSharedData(ITargetAnimationTagParameter parameter)
        {
            TargetAnimationTag = parameter.TargetAnimationTag;
        }

        public void CopyTo(ITargetAnimationTagParameter parameter)
        {
            parameter.TargetAnimationTag = TargetAnimationTag;
        }
    }
}
