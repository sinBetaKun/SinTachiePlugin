using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    public enum ZSortMode
    {
        [Display(Name = "Z座標完全無視", Description = "Z座標が描画順序に一切影響しない。")]
        Ignore,
        [Display(Name = "バススクリーン内", Description = "このモードに設定されている同バス内のパーツ同士で描画順序を、あくまでバス内で変える。「バス順に表示」モードのパーツをバス内のZ座標を0として描画する。")]
        BusScreen,
        [Display(Name = "グローバル空間内", Description = "このモードに設定されているパーツ同士で描画順序を変える。")]
        GlobalSpace,
    }
}
