using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum CenterPointMode
    {
        [Display(Name = nameof(TextResource.CenterPointMode_DontOverride), ResourceType = typeof(TextResource))]
        DontOverride,
        [Display(Name = nameof(TextResource.CenterPointMode_DontSet), ResourceType = typeof(TextResource))]
        DontSet,
        [Display(Name = nameof(TextResource.CenterPointMode_OnlyCoordinate), ResourceType = typeof(TextResource))]
        OnlyCoordinate,
        [Display(Name = nameof(TextResource.CenterPointMode_CustomPointName), ResourceType = typeof(TextResource))]
        CustomPointName,
    }
}
