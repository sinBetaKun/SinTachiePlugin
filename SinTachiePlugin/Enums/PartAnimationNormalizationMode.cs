using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.Enums
{
    /// <summary>
    /// ｱﾆﾒｰｼｮﾝﾚｲﾔｰの第一次出力（数値）をどう正規化するか。
    /// <br/>
    /// 第一次出力が文字列の場合役割を持たない。
    /// </summary>
    internal enum PartAnimationNormalizationMode
    {
        /// <summary>
        /// 第一出力値を区間[0,1]にクランプ（範囲外を端点に固定）し第二出力値とする。
        /// </summary>
        [Display(Name = nameof(TextResource.PartAnimationNormalizationMode_Limit), ResourceType = typeof(TextResource))]
        Limit,

        /// <summary>
        /// 第一出力値を区間[a,b]で端点で折り返し往復させ第二出力値とする。
        /// </summary>
        [Display(Name = nameof(TextResource.PartAnimationNormalizationMode_Shuttle), ResourceType = typeof(TextResource))]
        Shuttle,

        /// <summary>
        /// 第一出力値を区間(0,1]にラップ（範囲外から反対側へ循環）し第二出力値とする。
        /// </summary>
        [Display(Name = nameof(TextResource.PartAnimationNormalizationMode_Loop), ResourceType = typeof(TextResource))]
        Loop,

        /// <summary>
        /// 正規化モードの上書きを行わない。
        /// </summary>
        [Display(Name = nameof(TextResource.PartAnimationNormalizationMode_DontOverride), ResourceType = typeof(TextResource))]
        DontOverride,
    }
}
