using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    public enum OuterLayerValueMode
    {
        [Display(Name = nameof(Resources.Value_OutLyrVlMd_Limit), Description = nameof(Resources.Desc_OutLyrVlMd_Limit), ResourceType = typeof(Resources))]
        Limit,
        [Display(Name = nameof(Resources.Value_OutLyrVlMd_Shuttle), Description = nameof(Resources.Desc_OutLyrVlMd_Limit), ResourceType = typeof(Resources))]
        Shuttle,
        [Display(Name = nameof(Resources.Value_OutLyrVlMd_Loop), Description = nameof(Resources.Desc_OutLyrVlMd_Limit), ResourceType = typeof(Resources))]
        Loop,
    }
}
