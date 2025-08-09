using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.Enums
{
    internal enum PartAnimationLinkOpeMode
    {
        [Display(Name = nameof(Text.PartAnimationLink_LinkMode_Override), ResourceType = typeof(Text))]
        Override,
        [Display(Name = nameof(Text.PartAnimationLink_LinkMode_Add), ResourceType = typeof(Text))]
        Add,
        [Display(Name = nameof(Text.PartAnimationLink_LinkMode_Multiply), ResourceType = typeof(Text))]
        Multiply,
    }
}
