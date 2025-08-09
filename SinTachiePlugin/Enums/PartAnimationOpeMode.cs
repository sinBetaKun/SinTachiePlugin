using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.Enums
{
    internal enum PartAnimationOpeMode
    {
        [Display(Name = nameof(Text.PartAnimationOpeMode_Simple), ResourceType = typeof(Text))]
        Simple,
        [Display(Name = nameof(Text.PartAnimationOpeMode_Sum), ResourceType = typeof(Text))]
        Sum,
        [Display(Name = nameof(Text.PartAnimationOpeMode_Product), ResourceType = typeof(Text))]
        Product,
        [Display(Name = nameof(Text.PartAnimationOpeMode_Sin), ResourceType = typeof(Text))]
        Sin,
        [Display(Name = nameof(Text.PartAnimationOpeMode_VoiceVolume), ResourceType = typeof(Text))]
        VoiceVolume,
        [Display(Name = nameof(Text.PartAnimationOpeMode_VoiceVolumePlus), ResourceType = typeof(Text))]
        VoiceVolumePlus,
        [Display(Name = nameof(Text.PartAnimationOpeMode_PeriodicShuttle), ResourceType = typeof(Text))]
        PeriodicShuttle,
        [Display(Name = nameof(Text.PartAnimationOpeMode_PeriodicLoop), ResourceType = typeof(Text))]
        PeriodicLoop,
    }
}
