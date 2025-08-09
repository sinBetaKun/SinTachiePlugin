using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.EffectChainFlag
{
    [VideoEffect("ﾊﾟｰﾂ個別ｴﾌｪｸﾄﾁｪｲﾝﾌﾗｸﾞ", ["STP"], ["パーツ", "エフェクト" , "チェイン", "フラグ"], isAviUtlSupported: false)]
    public class SInTachiePlugin_PartEffectChainFlag : VideoEffectBase
    {
        /// <summary>
        /// エフェクトの名前
        /// </summary>
        public override string Label => $"[{FlagSign}] ﾊﾟｰﾂ個別ｴﾌｪｸﾄﾁｪｲﾝﾌﾗｸ";

        private string FlagSign => Flag ? "〇" : "×";

        [Display(GroupName = "以降のエフェクトを子パーツにも適応する", Name = "")]
        [ToggleSlider]
        public bool Flag { get => flag; set => Set(ref flag, value); }
        bool flag = false;

        /// <summary>
        /// Exoフィルタを作成する。
        /// </summary>
        /// <param name="keyFrameIndex">キーフレーム番号</param>
        /// <param name="exoOutputDescription">exo出力に必要な各種情報</param>
        /// <returns></returns>
        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            //サンプルはSampleD2DVideoEffectを参照
            return [];
        }

        /// <summary>
        /// 映像エフェクトを作成する
        /// </summary>
        /// <param name="devices">デバイス</param>
        /// <returns>映像エフェクト</returns>
        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new SInTachiePlugin_PartEffectChainFlag_Processor();
        }

        /// <summary>
        /// クラス内のIAnimatableを列挙する。
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IAnimatable> GetAnimatables() => [];
    }
}
