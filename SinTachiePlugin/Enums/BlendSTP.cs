using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    public enum BlendSTP
    {
        [Display(Name = nameof(Resources.Value_BlendSTP_SourceOver), ResourceType = typeof(Resources))]
        SourceOver,         // 普通
        [Display(Name = nameof(Resources.Value_BlendSTP_Dissolve), ResourceType = typeof(Resources))]
        Dissolve,           // ディザ合成
        [Display(Name = nameof(Resources.Value_BlendSTP_Darken), ResourceType = typeof(Resources))]
        Darken,             // 比較（暗）
        [Display(Name = nameof(Resources.Value_BlendSTP_Multiply), ResourceType = typeof(Resources))]
        Multiply,           // 乗算
        [Display(Name = nameof(Resources.Value_BlendSTP_ColorBurn), ResourceType = typeof(Resources))]
        ColorBurn,          // 焼きこみカラー
        [Display(Name = nameof(Resources.Value_BlendSTP_LinearBurn), ResourceType = typeof(Resources))]
        LinearBurn,         // 焼きこみ（リニア）
        [Display(Name = nameof(Resources.Value_BlendSTP_Lighten), ResourceType = typeof(Resources))]
        Lighten,            // 比較（明）
        [Display(Name = nameof(Resources.Value_BlendSTP_Screen), ResourceType = typeof(Resources))]
        Screen,             // スクリーン
        [Display(Name = nameof(Resources.Value_BlendSTP_ColorDodge), ResourceType = typeof(Resources))]
        ColorDodge,         // 覆い焼きカラー
        [Display(Name = nameof(Resources.Value_BlendSTP_LinearDodge), ResourceType = typeof(Resources))]
        LinearDodge,        // 覆い焼き（リニア）カラー
        [Display(Name = nameof(Resources.Value_BlendSTP_Plus), ResourceType = typeof(Resources))]
        Plus,               // 加算
        [Display(Name = nameof(Resources.Value_BlendSTP_Overlay), ResourceType = typeof(Resources))]
        Overlay,            // オーバーレイ
        [Display(Name = nameof(Resources.Value_BlendSTP_SoftLight), ResourceType = typeof(Resources))]
        SoftLight,          // ソフトライト
        [Display(Name = nameof(Resources.Value_BlendSTP_HardLight), ResourceType = typeof(Resources))]
        HardLight,          // ハードライト
        [Display(Name = nameof(Resources.Value_BlendSTP_VividLight), ResourceType = typeof(Resources))]
        VividLight,         // ビビッドライト
        [Display(Name = nameof(Resources.Value_BlendSTP_LinearLight), ResourceType = typeof(Resources))]
        LinearLight,        // リニアライト
        [Display(Name = nameof(Resources.Value_BlendSTP_PinLight), ResourceType = typeof(Resources))]
        PinLight,           // ピンライト
        [Display(Name = nameof(Resources.Value_BlendSTP_HardMix), ResourceType = typeof(Resources))]
        HardMix,            // ハードミックス
        [Display(Name = nameof(Resources.Value_BlendSTP_Difference), ResourceType = typeof(Resources))]
        Difference,         // 差分
        [Display(Name = nameof(Resources.Value_BlendSTP_Exclusion), ResourceType = typeof(Resources))]
        Exclusion,          // 除外
        [Display(Name = nameof(Resources.Value_BlendSTP_Subtract), ResourceType = typeof(Resources))]
        Subtract,           // 減算
        [Display(Name = nameof(Resources.Value_BlendSTP_Division), ResourceType = typeof(Resources))]
        Division,           // 除算
        [Display(Name = nameof(Resources.Value_BlendSTP_Hue), ResourceType = typeof(Resources))]
        Hue,                // 色相
        [Display(Name = nameof(Resources.Value_BlendSTP_Saturation), ResourceType = typeof(Resources))]
        Saturation,         // 彩度
        [Display(Name = nameof(Resources.Value_BlendSTP_Color), ResourceType = typeof(Resources))]
        Color,              // カラー
        [Display(Name = nameof(Resources.Value_BlendSTP_Luminosity), ResourceType = typeof(Resources))]
        Luminosity,         // 輝度
        [Display(Name = nameof(Resources.Value_BlendSTP_LighterColor), ResourceType = typeof(Resources))]
        LighterColor,       // カラー比較（明）
        [Display(Name = nameof(Resources.Value_BlendSTP_DestinationOver), ResourceType = typeof(Resources))]
        DestinationOver,    // 背景
        [Display(Name = nameof(Resources.Value_BlendSTP_DarkerColor), ResourceType = typeof(Resources))]
        DarkerColor,        // カラー比較（暗）
        [Display(Name = nameof(Resources.Value_BlendSTP_DestinationOut), ResourceType = typeof(Resources))]
        DestinationOut,     // 削除
        [Display(Name = nameof(Resources.Value_BlendSTP_SourceAtop), ResourceType = typeof(Resources))]
        SourceAtop,         // 背景でクリッピング
        [Display(Name = nameof(Resources.Value_BlendSTP_XOR), ResourceType = typeof(Resources))]
        XOR,                // 重ならない部分のみ
        [Display(Name = nameof(Resources.Value_BlendSTP_MaskInverseErt), ResourceType = typeof(Resources))]
        MaskInverseErt,     // 色反転マスク
    }
}
