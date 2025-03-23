using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    public enum BlendSTP
    {
        [Display(Name = nameof(Resources.Value_BlendSTP_SourceOver), ResourceType = typeof(Resources))]
        SourceOver,
        [Display(Name = nameof(Resources.Value_BlendSTP_Dissolve), ResourceType = typeof(Resources))]
        Dissolve,
        [Display(Name = nameof(Resources.Value_BlendSTP_Darken), ResourceType = typeof(Resources))]
        Darken,
        [Display(Name = nameof(Resources.Value_BlendSTP_Multiply), ResourceType = typeof(Resources))]
        Multiply,
        [Display(Name = nameof(Resources.Value_BlendSTP_ColorBurn), ResourceType = typeof(Resources))]
        ColorBurn,
        [Display(Name = nameof(Resources.Value_BlendSTP_LinearBurn), ResourceType = typeof(Resources))]
        LinearBurn,
        [Display(Name = nameof(Resources.Value_BlendSTP_Lighten), ResourceType = typeof(Resources))]
        Lighten,
        [Display(Name = nameof(Resources.Value_BlendSTP_Screen), ResourceType = typeof(Resources))]
        Screen,
        [Display(Name = nameof(Resources.Value_BlendSTP_ColorDodge), ResourceType = typeof(Resources))]
        ColorDodge,
        [Display(Name = nameof(Resources.Value_BlendSTP_LinearDodge), ResourceType = typeof(Resources))]
        LinearDodge,
        [Display(Name = nameof(Resources.Value_BlendSTP_Plus), ResourceType = typeof(Resources))]
        Plus,
        [Display(Name = nameof(Resources.Value_BlendSTP_Overlay), ResourceType = typeof(Resources))]
        Overlay,
        [Display(Name = nameof(Resources.Value_BlendSTP_SoftLight), ResourceType = typeof(Resources))]
        SoftLight,
        [Display(Name = nameof(Resources.Value_BlendSTP_HardLight), ResourceType = typeof(Resources))]
        HardLight,
        [Display(Name = nameof(Resources.Value_BlendSTP_VividLight), ResourceType = typeof(Resources))]
        VividLight,
        [Display(Name = nameof(Resources.Value_BlendSTP_LinearLight), ResourceType = typeof(Resources))]
        LinearLight,
        [Display(Name = nameof(Resources.Value_BlendSTP_PinLight), ResourceType = typeof(Resources))]
        PinLight,
        [Display(Name = nameof(Resources.Value_BlendSTP_HardMix), ResourceType = typeof(Resources))]
        HardMix,
        [Display(Name = nameof(Resources.Value_BlendSTP_Difference), ResourceType = typeof(Resources))]
        Difference,
        [Display(Name = nameof(Resources.Value_BlendSTP_Exclusion), ResourceType = typeof(Resources))]
        Exclusion,
        [Display(Name = nameof(Resources.Value_BlendSTP_Subtract), ResourceType = typeof(Resources))]
        Subtract,
        [Display(Name = nameof(Resources.Value_BlendSTP_Division), ResourceType = typeof(Resources))]
        Division,
        [Display(Name = nameof(Resources.Value_BlendSTP_Hue), ResourceType = typeof(Resources))]
        Hue,
        [Display(Name = nameof(Resources.Value_BlendSTP_Saturation), ResourceType = typeof(Resources))]
        Saturation,
        [Display(Name = nameof(Resources.Value_BlendSTP_Color), ResourceType = typeof(Resources))]
        Color,
        [Display(Name = nameof(Resources.Value_BlendSTP_Luminosity), ResourceType = typeof(Resources))]
        Luminosity,
        [Display(Name = nameof(Resources.Value_BlendSTP_LighterColor), ResourceType = typeof(Resources))]
        LighterColor,
        [Display(Name = nameof(Resources.Value_BlendSTP_DestinationOver), ResourceType = typeof(Resources))]
        DestinationOver,
        [Display(Name = nameof(Resources.Value_BlendSTP_DarkerColor), ResourceType = typeof(Resources))]
        DarkerColor,
        [Display(Name = nameof(Resources.Value_BlendSTP_DestinationOut), ResourceType = typeof(Resources))]
        DestinationOut,
        [Display(Name = nameof(Resources.Value_BlendSTP_SourceAtop), ResourceType = typeof(Resources))]
        SourceAtop,
        [Display(Name = nameof(Resources.Value_BlendSTP_XOR), ResourceType = typeof(Resources))]
        XOR,
        [Display(Name = nameof(Resources.Value_BlendSTP_MaskInverseErt), ResourceType = typeof(Resources))]
        MaskInverseErt,
    }
}
