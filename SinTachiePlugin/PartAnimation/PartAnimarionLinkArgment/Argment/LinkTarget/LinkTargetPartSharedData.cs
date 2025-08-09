namespace SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Argment.LinkTarget
{
    internal class LinkTargetPartSharedData
    {
        public string LinkTarget { get; set; } = string.Empty;

        public LinkTargetPartSharedData()
        {
        }

        public LinkTargetPartSharedData(ILinkTargetPartParameter parameter)
        {
            LinkTarget = parameter.LinkTargetPart;
        }

        public void CopyTo(ILinkTargetPartParameter parameter)
        {
            parameter.LinkTargetPart = LinkTarget;
        }
    }
}
