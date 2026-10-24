using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.Draw.EffectChain
{
    internal class SubChainNode : IDisposable
    {
        public readonly IVideoEffect Effect;
        public ID2D1Image Output => _proseccor.Output;
        private readonly IVideoEffectProcessor _proseccor;
        
        public SubChainNode(IGraphicsDevicesAndContext devices, IVideoEffect effect)
        {
            Effect = effect;
            _proseccor = effect.CreateVideoEffect(devices);
        }

        public void SetInput(ID2D1Image input)
        {
            _proseccor.SetInput(input);
        }

        public DrawDescription Update(EffectDescription desc)
        {
            return _proseccor.Update(desc);
        }

        public void Dispose()
        {
            _proseccor.ClearInput();
            _proseccor.Dispose();
        }
    }
}
