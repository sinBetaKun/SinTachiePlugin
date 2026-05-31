namespace SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Argument.TargetPartTag
{
    internal class TargetPartTagSharedData
    {
        public string TargetPartTag { get; set; } = string.Empty;

        public TargetPartTagSharedData()
        {
        }

        public TargetPartTagSharedData(ITargetPartTagParameter parameter)
        {
            TargetPartTag = parameter.TargetPartTag;
        }

        public void CopyTo(ITargetPartTagParameter parameter)
        {
            parameter.TargetPartTag = TargetPartTag;
        }
    }
}
