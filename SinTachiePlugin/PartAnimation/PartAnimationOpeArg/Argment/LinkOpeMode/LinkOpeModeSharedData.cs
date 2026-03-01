using SinTachiePlugin.Enums;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.LinkOpeMode
{
    internal class LinkOpeModeSharedData
    {
        public PartAnimationLinkOpeMode LinkOpeMode { get; set; } = PartAnimationLinkOpeMode.DontLink;

        public LinkOpeModeSharedData()
        {
        }

        public LinkOpeModeSharedData(ILinkOpeModeParameter parameter)
        {
            LinkOpeMode = parameter.LinkOpeMode;
        }

        public void CopyTo(ILinkOpeModeParameter parameter)
        {
            parameter.LinkOpeMode = LinkOpeMode;
        }
    }
}
