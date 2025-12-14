using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum CenterPointMode
    {
        [Display(Name = nameof(TextResource.CenterPointMode_OfPart), ResourceType = typeof(TextResource))]
        OfPart,
        [Display(Name = nameof(TextResource.CenterPointMode_OfImage), ResourceType = typeof(TextResource))]
        OfImage,
        [Display(Name = nameof(TextResource.CenterPointMode_CustomPointName), ResourceType = typeof(TextResource))]
        CustomPointName,
        [Display(Name = nameof(TextResource.CenterPointMode_DontOverride), ResourceType = typeof(TextResource))]
        DontOverride,
    }
}
