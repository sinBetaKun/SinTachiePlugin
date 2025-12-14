using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.Enums
{
    public enum LayerType
    {
        [Display(Name = nameof(TextResource.LayerType_Image), ResourceType = typeof(TextResource))]
        Image,
        [Display(Name = nameof(TextResource.LayerType_Psd), ResourceType = typeof(TextResource))]
        Psd,
        [Display(Name = nameof(TextResource.LayerType_Video), ResourceType = typeof(TextResource))]
        Video,
        [Display(Name = nameof(TextResource.LayerType_Scene), ResourceType = typeof(TextResource))]
        Scene,
        [Display(Name = nameof(TextResource.LayerType_Group), ResourceType = typeof(TextResource))]
        Group,
    }
}
