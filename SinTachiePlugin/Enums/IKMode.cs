using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum IKMode
    {
        [Display(Name = nameof(TextResource.IKMode_None), ResourceType = typeof(TextResource))]
        None,
        [Display(Name = nameof(TextResource.IKMode_TypeA), ResourceType = typeof(TextResource))]
        IKMode_TypeA,
        [Display(Name = nameof(TextResource.IKMode_DontOverride), ResourceType = typeof(TextResource))]
        DontOverride,
    }
}
