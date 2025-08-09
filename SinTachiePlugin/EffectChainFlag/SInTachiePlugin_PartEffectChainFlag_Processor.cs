using Vortice.Direct2D1;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.EffectChainFlag
{
    internal class SInTachiePlugin_PartEffectChainFlag_Processor : IVideoEffectProcessor
    {
        private ID2D1Image? input;

        public ID2D1Image Output => input ?? throw new ArgumentNullException("入力がありません");

        public SInTachiePlugin_PartEffectChainFlag_Processor()
        {
        }

        public void SetInput(ID2D1Image? input)
        {
            this.input = input;
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            return effectDescription.DrawDescription;
        }

        public void ClearInput()
        {
        }

        public void Dispose()
        {
        }
    }
}
