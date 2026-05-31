using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.Enums
{
    internal enum PartAnimationOpeMode
    {
        [Display(Name = nameof(TextResource.PartAnimationOpeMode_Simple), ResourceType = typeof(TextResource))]
        Simple,
        [Display(Name = nameof(TextResource.PartAnimationOpeMode_Sum), ResourceType = typeof(TextResource))]
        Sum,
        [Display(Name = nameof(TextResource.PartAnimationOpeMode_Product), ResourceType = typeof(TextResource))]
        Product,
        [Display(Name = nameof(TextResource.PartAnimationOpeMode_Sin), ResourceType = typeof(TextResource))]
        Sin,
        [Display(Name = nameof(TextResource.PartAnimationOpeMode_VoiceVolume), ResourceType = typeof(TextResource))]
        VoiceVolume,
        [Display(Name = nameof(TextResource.PartAnimationOpeMode_VoiceVolumePlus), ResourceType = typeof(TextResource))]
        VoiceVolumePlus,
        [Display(Name = nameof(TextResource.PartAnimationOpeMode_AIUEOMouth), ResourceType = typeof(TextResource))]
        AIUEOMouth,
        [Display(Name = nameof(TextResource.PartAnimationOpeMode_PeriodicShuttle), ResourceType = typeof(TextResource))]
        PeriodicShuttle,
        [Display(Name = nameof(TextResource.PartAnimationOpeMode_PeriodicLoop), ResourceType = typeof(TextResource))]
        PeriodicLoop,
        [Display(Name = nameof(TextResource.PartAnimationOpeMode_RandomShuttle), ResourceType = typeof(TextResource))]
        RandomShuttle,
        [Display(Name = nameof(TextResource.PartAnimationOpeMode_RandomLoop), ResourceType = typeof(TextResource))]
        RandomLoop,
        [Display(Name = nameof(TextResource.PartAnimationOpeMode_PeriodicLoop), ResourceType = typeof(TextResource))]
        AudioFile,
        [Display(Name = nameof(TextResource.PartAnimationOpeMode_StringInput), ResourceType = typeof(TextResource))]
        StringInput,
    }
}
