using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum DrawingValueMode
    {
        [Display(Name = nameof(Texts.ValueMode_Override), Description = nameof(Texts.ValueMode_Override_Description), ResourceType = typeof(Texts))]
        Override,
        [Display(Name = nameof(Texts.ValueMode_Compose), Description = nameof(Texts.ValueMode_Compose_Description), ResourceType = typeof(Texts))]
        Compose,
    }
}
