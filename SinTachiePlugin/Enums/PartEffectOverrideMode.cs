using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum PartEffectOverrideMode
    {
        [Display(Name = nameof(TextResource.PartEffectOverrideMode_Add), ResourceType = typeof(TextResource))]
        Add,
        [Display(Name = nameof(TextResource.PartEffectOverrideMode_Override), ResourceType = typeof(TextResource))]
        Override,
    }
}
