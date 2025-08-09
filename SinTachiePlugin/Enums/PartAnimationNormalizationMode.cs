using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.Enums
{
    internal enum PartAnimationNormalizationMode
    {
        [Display(Name = nameof(Text.PartAnimationNormalizationMode_Limit), ResourceType = typeof(Text))]
        Limit,
        [Display(Name = nameof(Text.PartAnimationNormalizationMode_Shuttle), ResourceType = typeof(Text))]
        Shuttle,
        [Display(Name = nameof(Text.PartAnimationNormalizationMode_Loop), ResourceType = typeof(Text))]
        Loop,
    }
}
