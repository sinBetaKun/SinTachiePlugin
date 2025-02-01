using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    public enum OuterLayerValueMode
    {
        [Display(Name = "指定なし", Description = "0%~100%の範囲内でのみ変化\n 0%以下の時はセラール、100%以上の時はアブリールの値になる。")]
        Limit,
        [Display(Name = "往復", Description = "100%ごとに実際の動きが反転する。\n例) ..⇄-200%(ｾﾗｰﾙ) ⇄ -100%(ｱﾌﾞﾘｰﾙ) ⇄ 0%(ｾﾗｰﾙ) ⇄ 100%(ｱﾌﾞﾘｰﾙ) ⇄ 200%(ｾﾗｰﾙ)⇄..")]
        Shuttle,
        [Display(Name = "ループ", Description = "100%ごとに同じ動きを繰り返す。\n例) ..| -100%(ｾﾗｰﾙ) ⇄ -0.1%(ｱﾌﾞﾘｰﾙ) | 0%(ｾﾗｰﾙ) ⇄ 99.9%(ｱﾌﾞﾘｰﾙ) | 100%(ｾﾗｰﾙ) ⇄ 199.9%(ｱﾌﾞﾘｰﾙ) |..")]
        Loop,
    }
}
