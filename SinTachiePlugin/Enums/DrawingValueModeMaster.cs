using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum DrawingValueModeMaster
    {
        [Display(Name = nameof(TextResource.ValueMode_Override), Description = nameof(TextResource.ValueMode_Override_Description), ResourceType = typeof(TextResource))]
        Override,
        [Display(Name = nameof(TextResource.ValueMode_Compose), Description = nameof(TextResource.ValueMode_Compose_Description), ResourceType = typeof(TextResource))]
        Compose,
        [Display(Name = nameof(TextResource.ValueMode_Custom), Description = nameof(TextResource.ValueMode_Custom_Description), ResourceType = typeof(TextResource))]
        Custom,
        [Display(Name = nameof(TextResource.ValueMode_DontOverride), Description = nameof(TextResource.ValueMode_Override_Description), ResourceType = typeof(TextResource))]
        DontOverride,
    }
}
