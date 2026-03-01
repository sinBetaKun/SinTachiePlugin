using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum PartAnimationValueMode
    {
        [Display(Name = nameof(TextResource.PartAnimationValueMode_None), ResourceType = typeof(TextResource))]
        None,
        [Display(Name = nameof(TextResource.PartAnimationValueMode_Single), ResourceType = typeof(TextResource))]
        Single,
        [Display(Name = nameof(TextResource.PartAnimationValueMode_Multi), ResourceType = typeof(TextResource))]
        Multi,
    }
}
