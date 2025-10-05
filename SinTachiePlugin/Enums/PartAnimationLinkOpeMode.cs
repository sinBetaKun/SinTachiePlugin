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
        [Display(Name = nameof(Texts.PartAnimationLink_LinkOpeMode_DontLink), ResourceType = typeof(Texts))]
        DontLink,
        [Display(Name = nameof(Texts.PartAnimationLink_LinkOpeMode_Add), ResourceType = typeof(Texts))]
        Add,
        [Display(Name = nameof(Texts.PartAnimationLink_LinkOpeMode_Multiply), ResourceType = typeof(Texts))]
        Multiply,
    }
}
