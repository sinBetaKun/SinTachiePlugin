using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.Enums
{
    internal enum ZSortMode2
    {
        [Display(Name = nameof(Texts.ZSortMode_IgnoreZ), ResourceType = typeof(Texts))]
        IgnoreZ,
        [Display(Name = nameof(Texts.ZSortMode_InGroup), ResourceType = typeof(Texts))]
        InGroup,
        [Display(Name = nameof(Texts.ZSortMode_IgnoreGroup), ResourceType = typeof(Texts))]
        IgnoreGroup,
    }
}
