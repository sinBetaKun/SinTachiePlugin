using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum ClippingMode
    {
        [Display(Name = nameof(TextResource.ClippingMode_DontClip), ResourceType = typeof(TextResource))]
        DontClip,
        [Display(Name = nameof(TextResource.ClippingMode_ClipToParent), ResourceType = typeof(TextResource))]
        ClipToParent,
        [Display(Name = nameof(TextResource.ClippingMode_ClipWithTag), ResourceType = typeof(TextResource))]
        ClipWithTag,
        [Display(Name = nameof(TextResource.GlobalEnumDisplay_DontOverride), ResourceType = typeof(TextResource))]
        DontOverride,
    }
}
