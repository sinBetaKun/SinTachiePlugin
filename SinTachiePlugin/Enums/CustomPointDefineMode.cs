using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum CustomPointDefineMode
    {
        [Display(Name = nameof(TextResource.CustomPointDefineMode_DontMake), ResourceType = typeof(TextResource))]
        DontMake,
        [Display(Name = nameof(TextResource.CustomPointDefineMode_DoMake), ResourceType = typeof(TextResource))]
        DoMake,
    }
}
