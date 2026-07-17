using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum PartInsertPosition
    {
        [Display(Name = nameof(TextResource.PartInsertPosition_Back), ResourceType = typeof(TextResource))]
        Back,
        [Display(Name = nameof(TextResource.PartInsertPosition_Front), ResourceType = typeof(TextResource))]
        Front,
    }
}
