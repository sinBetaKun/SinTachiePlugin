using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    public enum ZSortMode
    {
        [Display(Name = nameof(Resources.Value_ZSortMode_Ignore), Description = nameof(Resources.Desc_ZSortMode_Ignore), ResourceType = typeof(Resources))]
        Ignore,
        [Display(Name = nameof(Resources.Value_ZSortMode_BusScreen), Description = nameof(Resources.Desc_ZSortMode_BusScreen), ResourceType = typeof(Resources))]
        BusScreen,
        [Display(Name = nameof(Resources.Value_ZSortMode_GlobalSpace), Description = nameof(Resources.Desc_ZSortMode_GlobalSpace), ResourceType = typeof(Resources))]
        GlobalSpace,
    }
}
