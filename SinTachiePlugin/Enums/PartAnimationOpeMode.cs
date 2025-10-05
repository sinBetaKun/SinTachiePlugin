using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.Enums
{
    internal enum PartAnimationOpeMode
    {
        [Display(Name = nameof(Texts.PartAnimationOpeMode_Simple), ResourceType = typeof(Texts))]
        Simple,
        [Display(Name = nameof(Texts.PartAnimationOpeMode_Sum), ResourceType = typeof(Texts))]
        Sum,
        [Display(Name = nameof(Texts.PartAnimationOpeMode_Product), ResourceType = typeof(Texts))]
        Product,
        [Display(Name = nameof(Texts.PartAnimationOpeMode_Sin), ResourceType = typeof(Texts))]
        Sin,
        [Display(Name = nameof(Texts.PartAnimationOpeMode_VoiceVolume), ResourceType = typeof(Texts))]
        VoiceVolume,
        [Display(Name = nameof(Texts.PartAnimationOpeMode_VoiceVolumePlus), ResourceType = typeof(Texts))]
        VoiceVolumePlus,
        [Display(Name = nameof(Texts.PartAnimationOpeMode_PeriodicShuttle), ResourceType = typeof(Texts))]
        PeriodicShuttle,
        [Display(Name = nameof(Texts.PartAnimationOpeMode_PeriodicLoop), ResourceType = typeof(Texts))]
        PeriodicLoop,
        [Display(Name = nameof(Texts.PartAnimationOpeMode_PeriodicLoop), ResourceType = typeof(Texts))]
        AudioFile,
    }
}
