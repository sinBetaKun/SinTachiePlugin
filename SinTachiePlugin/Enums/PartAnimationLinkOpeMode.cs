using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.Enums
{
    internal enum PartAnimationLinkOpeMode
    {
        [Display(Name = nameof(TextResource.PartAnimationLink_LinkOpeMode_DontLink), ResourceType = typeof(TextResource))]
        DontLink,
        [Display(Name = nameof(TextResource.PartAnimationLink_LinkOpeMode_Add), ResourceType = typeof(TextResource))]
        Add,
        [Display(Name = nameof(TextResource.PartAnimationLink_LinkOpeMode_Multiply), ResourceType = typeof(TextResource))]
        Multiply,
        [Display(Name = nameof(TextResource.PartAnimationLink_LinkOpeMode_DontOverride), ResourceType = typeof(TextResource))]
        DontOverride
    }
}
