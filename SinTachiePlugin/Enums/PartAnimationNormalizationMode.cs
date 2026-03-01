using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.Enums
{
    internal enum PartAnimationNormalizationMode
    {
        [Display(Name = nameof(TextResource.PartAnimationNormalizationMode_Limit), ResourceType = typeof(TextResource))]
        Limit,
        [Display(Name = nameof(TextResource.PartAnimationNormalizationMode_Shuttle), ResourceType = typeof(TextResource))]
        Shuttle,
        [Display(Name = nameof(TextResource.PartAnimationNormalizationMode_Loop), ResourceType = typeof(TextResource))]
        Loop,
    }
}
