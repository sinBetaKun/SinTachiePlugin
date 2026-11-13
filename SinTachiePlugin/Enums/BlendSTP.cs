using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinTachiePlugin.Enums
{
    public enum BlendSTP
    {
        [Display(Name = "普通", Description = "普通")]
        SourceOver,
        [Display(Name = "ディザ合成", Description = "ディザ合成")]
        Dissolve,
        [Display(Name = "比較（暗）", Description = "比較（暗）")]
        Darken,
        [Display(Name = "乗算", Description = "乗算")]
        Multiply,
        [Display(Name = "焼き込みカラー", Description = "焼き込みカラー")]
        ColorBurn,
        [Display(Name = "焼き込み（リニア）", Description = "焼き込み（リニア）")]
        LinearBurn,
        [Display(Name = "比較（明）", Description = "比較（明）")]
        Lighten,
        [Display(Name = "スクリーン", Description = "スクリーン")]
        Screen,
        [Display(Name = "覆い焼きカラー", Description = "覆い焼きカラー")]
        ColorDodge,
        [Display(Name = "覆い焼き（リニア）-加算", Description = "覆い焼き（リニア）-加算")]
        LinearDodge,
        [Display(Name = "加算", Description = "加算")]
        Plus,
        [Display(Name = "オーバーレイ", Description = "オーバーレイ")]
        Overlay,
        [Display(Name = "ソフトライト", Description = "ソフトライト")]
        SoftLight,
        [Display(Name = "ハードライト", Description = "ハードライト")]
        HardLight,
        [Display(Name = "ビビッドライト", Description = "ビビッドライト")]
        VividLight,
        [Display(Name = "リニアライト", Description = "リニアライト")]
        LinearLight,
        [Display(Name = "ピンライト", Description = "ピンライト")]
        PinLight,
        [Display(Name = "ハードミックス", Description = "ハードミックス")]
        HardMix,
        [Display(Name = "差分", Description = "差分")]
        Difference,
        [Display(Name = "除外", Description = "除外")]
        Exclusion,
        [Display(Name = "減算", Description = "減算")]
        Subtract,
        [Display(Name = "除算", Description = "除算")]
        Division,
        [Display(Name = "色相", Description = "色相")]
        Hue,
        [Display(Name = "彩度", Description = "彩度")]
        Saturation,
        [Display(Name = "カラー", Description = "カラー")]
        Color,
        [Display(Name = "輝度", Description = "輝度")]
        Luminosity,
        [Display(Name = "カラー比較（明）", Description = "カラー比較（明）")]
        LighterColor,
        [Display(Name = "背景", Description = "背景")]
        DestinationOver,
        [Display(Name = "カラー比較（暗）", Description = "カラー比較（暗）")]
        DarkerColor,
        [Display(Name = "削除", Description = "削除")]
        DestinationOut,
        [Display(Name = "背景でクリッピング", Description = "背景でクリッピング")]
        SourceAtop,
        [Display(Name = "重ならない部分のみ", Description = "重ならない部分のみ")]
        XOR,
        [Display(Name = "反転マスク", Description = "反転マスク")]
        MaskInverseErt,
    }
}
