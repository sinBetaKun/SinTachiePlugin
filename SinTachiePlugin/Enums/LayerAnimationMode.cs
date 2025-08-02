using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    public enum LayerAnimationMode
    {
        [Display(Name = nameof(Resources.Value_LyrAnmMd_CerrarPlusAbrir), Description = nameof(Resources.Desc_LyrAnmMd_CerrarPlusAbrir), ResourceType = typeof(Resources))]
        CerrarPlusAbrir,
        [Display(Name = nameof(Resources.Value_LyrAnmMd_CerrarTimesAbrir), Description = nameof(Resources.Desc_LyrAnmMd_CerrarTimesAbrir), ResourceType = typeof(Resources))]
        CerrarTimesAbrir,
        [Display(Name = nameof(Resources.Value_LyrAnmMd_Sin), Description = nameof(Resources.Desc_LyrAnmMd_Sin), ResourceType = typeof(Resources))]
        Sin,
        [Display(Name = nameof(Resources.Value_LyrAnmMd_VoiceVolume), Description = nameof(Resources.Desc_LyrAnmMd_VoiceVolume), ResourceType = typeof(Resources))]
        VoiceVolume,
        [Display(Name = nameof(Resources.Value_LyrAnmMd_PeriodicShuttle), Description = nameof(Resources.Desc_LyrAnmMd_PeriodicShuttle), ResourceType = typeof(Resources))]
        PeriodicShuttle,
        [Display(Name = nameof(Resources.Value_LyrAnmMd_PeriodicLoop), Description = nameof(Resources.Desc_LyrAnmMd_PeriodicLoop), ResourceType = typeof(Resources))]
        PeriodicLoop,
    }
}
