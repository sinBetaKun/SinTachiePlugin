using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum OriginDefineMode
    {
        [Display(Name = nameof(TextResource.OriginDefineMode_DontOverride), ResourceType = typeof(TextResource))]
        DontOverride,
        [Display(Name = nameof(TextResource.OriginDefineMode_CenterOfParent), ResourceType = typeof(TextResource))]
        CenterOfParent,
        [Display(Name = nameof(TextResource.OriginDefineMode_CenterOfParentSource), ResourceType = typeof(TextResource))]
        CenterOfParentSource,
        [Display(Name = nameof(TextResource.OriginDefineMode_CustomPointOfParent), ResourceType = typeof(TextResource))]
        CustomPointOfParent,
    }
}
