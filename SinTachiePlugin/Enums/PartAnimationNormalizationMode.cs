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
        [Display(Name = nameof(Texts.PartAnimationNormalizationMode_Limit), ResourceType = typeof(Texts))]
        Limit,
        [Display(Name = nameof(Texts.PartAnimationNormalizationMode_Shuttle), ResourceType = typeof(Texts))]
        Shuttle,
        [Display(Name = nameof(Texts.PartAnimationNormalizationMode_Loop), ResourceType = typeof(Texts))]
        Loop,
    }
}
