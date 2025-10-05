using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.Enums
{
    internal enum LayerType
    {
        [Display(Name = nameof(Texts.LayerType_Image), ResourceType = typeof(Texts))]
        Image,
        [Display(Name = nameof(Texts.LayerType_Psd), ResourceType = typeof(Texts))]
        Psd,
        [Display(Name = nameof(Texts.LayerType_Video), ResourceType = typeof(Texts))]
        Video,
        [Display(Name = nameof(Texts.LayerType_Scene), ResourceType = typeof(Texts))]
        Scene,
        [Display(Name = nameof(Texts.LayerType_Group), ResourceType = typeof(Texts))]
        Group,
        [Display(Name = nameof(Texts.LayerType_Group_CompressFrame), ResourceType = typeof(Texts))]
        Group_CompressFrame,
    }
}
