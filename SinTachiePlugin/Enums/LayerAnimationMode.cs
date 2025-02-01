using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Parts.LayerValueListController
{
    public enum LayerAnimationMode
    {
        [Display(Name = "２数の和", Description = "「セラール」と「アブリール」の値を足した結果でレイヤーを制御します。")]
        CerrarPlusAbrir,
        [Display(Name = "２数の積", Description = "「セラール」と「アブリール」の値を掛けた結果でレイヤーを制御します。")]
        CerrarTimesAbrir,
        [Display(Name = "sin", Description = "「セラール」を振幅、「アブリール」を位相とするsin波でレイヤーを制御します。")]
        Sin,
        [Display(Name = "口パク", Description = "（セラール）～（アブリール）の範囲で口パクします。\n口が閉じるときに(セラール)の値に、\n全開のときに(アブリール)の値になります。")]
        VoiceVolume,
        [Display(Name = "周期的往復", Description = "（セラール）～（アブリール）の範囲で「往復」と同じ動きを定期的にします。\n(セラール)の値と(アブリール)の値の間を周期的に往復します。")]
        PeriodicShuttle,
        [Display(Name = "周期的ループ", Description = "（セラール）～（アブリール）の範囲で「ループ」と同じ動きを定期的にします。\n(セラール)の値と(アブリール)の値の間を周期的に往復します。")]
        PeriodicLoop,
    }
}
