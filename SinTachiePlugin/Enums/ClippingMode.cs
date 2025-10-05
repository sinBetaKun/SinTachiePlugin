using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum ClippingMode
    {
        [Display(Name = nameof(Texts.ClippingMode_DontClip), ResourceType = typeof(Texts))]
        DontClip,
        [Display(Name = nameof(Texts.ClippingMode_ClipToParent), ResourceType = typeof(Texts))]
        ClipToParent,
        [Display(Name = nameof(Texts.ClippingMode_ClipWithTag), ResourceType = typeof(Texts))]
        ClipWithTag,
    }
}
