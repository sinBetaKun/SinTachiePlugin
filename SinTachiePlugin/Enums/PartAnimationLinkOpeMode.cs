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
        [Display(Name = nameof(TextResource.PartAnimationLink_LinkOpeMode_DontLink), ResourceType = typeof(TextResource))]
        DontLink,
        [Display(Name = nameof(TextResource.PartAnimationLink_LinkOpeMode_Add), ResourceType = typeof(TextResource))]
        Add,
        [Display(Name = nameof(TextResource.PartAnimationLink_LinkOpeMode_Multiply), ResourceType = typeof(TextResource))]
        Multiply,
    }
}
