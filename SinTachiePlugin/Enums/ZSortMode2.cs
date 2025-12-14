using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.Enums
{
    internal enum ZSortMode2
    {
        [Display(Name = nameof(TextResource.ZSortMode_IgnoreZ), ResourceType = typeof(TextResource))]
        IgnoreZ,
        [Display(Name = nameof(TextResource.ZSortMode_BasedOnPriority), ResourceType = typeof(TextResource))]
        BasedOnPriority,
        [Display(Name = nameof(TextResource.ZSortMode_IgnorePriority), ResourceType = typeof(TextResource))]
        IgnorePriority,
        [Display(Name = nameof(TextResource.ZSortMode_DontOverride), ResourceType = typeof(TextResource))]
        DontOverride,
    }
}
