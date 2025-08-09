namespace SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Argment.LinkTag
{
    internal class LinkTagSharedData
    {
        public string LinkTag { get; set; } = string.Empty;

        public LinkTagSharedData()
        {
        }

        public LinkTagSharedData(ILinkTagParameter parameter)
        {
            LinkTag = parameter.LinkTag;
        }

        public void CopyTo(ILinkTagParameter parameter)
        {
            parameter.LinkTag = LinkTag;
        }
    }
}
