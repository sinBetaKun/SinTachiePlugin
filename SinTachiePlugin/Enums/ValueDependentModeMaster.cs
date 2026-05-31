using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum ValueDependentModeMaster
    {
        [Display(Name = nameof(TextResource.ValueDependentMode_On), ResourceType = typeof(TextResource))]
        On,
        [Display(Name = nameof(TextResource.ValueDependentMode_Off), ResourceType = typeof(TextResource))]
        Off,
        [Display(Name = nameof(TextResource.ValueDependentMode_Custom), ResourceType = typeof(TextResource))]
        Custom,
        [Display(Name = nameof(TextResource.ValueDependentMode_DontOverride), ResourceType = typeof(TextResource))]
        DontOverride
    }
}
