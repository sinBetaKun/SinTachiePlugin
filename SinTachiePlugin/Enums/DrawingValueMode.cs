using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum DrawingValueMode
    {
        [Display(Name = nameof(TextResource.ValueMode_Override), Description = nameof(TextResource.ValueMode_Override_Description), ResourceType = typeof(TextResource))]
        Override,
        [Display(Name = nameof(TextResource.ValueMode_Compose), Description = nameof(TextResource.ValueMode_Compose_Description), ResourceType = typeof(TextResource))]
        Compose,
    }
}
