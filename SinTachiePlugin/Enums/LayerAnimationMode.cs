using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
