using SinTachiePlugin.Enums;
using SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Parameter;

namespace SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment
{
    internal static class PartAnimationLinkOpeModeEx
    {
        public static PartAnimationLinkArgBase Convert(this PartAnimationLinkOpeMode mode, PartAnimationLinkArgBase current)
        {
            var store = current.GetSharedData();
            PartAnimationLinkArgBase param = mode switch 
            {
                PartAnimationLinkOpeMode.DontLink => new DontLinkParameter(store),
                PartAnimationLinkOpeMode.Add => new DoLinkParameter(store),
                PartAnimationLinkOpeMode.Multiply => new DoLinkParameter(store),
                PartAnimationLinkOpeMode.DontOverride => new DontLinkParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };

            if (param.GetType() != current.GetType())
                return param;
            return current;
        }
    }
}
