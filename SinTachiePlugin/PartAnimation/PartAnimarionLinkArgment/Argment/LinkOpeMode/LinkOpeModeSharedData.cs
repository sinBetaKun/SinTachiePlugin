using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinTachiePlugin.Enums;

namespace SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Argment.LinkOpeMode
{
    internal class LinkOpeModeSharedData
    {
        public PartAnimationLinkOpeMode LinkOpeMode { get; set; } = PartAnimationLinkOpeMode.Override;

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
