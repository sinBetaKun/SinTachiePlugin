using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.Draw.EffectChain
{
    internal class VideoEffectChainNode : IDisposable
    {
        public readonly PartNode Pn;
        private readonly IGraphicsDevicesAndContext _devices;
        private readonly ID2D1Bitmap _empty;
        private SubChainNode[] _chain = [];

        public ID2D1Image Output { get; private set; }

        public VideoEffectChainNode(IGraphicsDevicesAndContext devices, PartNode pn)
        {
            Pn = pn;
            _devices = devices;
            _empty = devices.DeviceContext.CreateEmptyBitmap();
            Output = _empty;
        }

        public DrawDescription Update(IEnumerable<PartEffectsAndFL> pefls, ID2D1Image input, TimelineSourceDescription tsDesc, DrawDescription dDesc)
        {
            Output = _empty;
            PartEffectsAndFL[] a0 = [.. pefls];
            SubChainNode[] a1 = [.. _chain.Where(x => a0.FirstOrDefault(y => y.Effects.Contains(x.Effect)) is null)];
            List<SubChainNode> l0 = [];
            DrawDescription _0 = dDesc;
            ID2D1Image _1 = input;

            foreach (PartEffectsAndFL _2 in a0)
            {
                TimelineItemSourceDescription _3 = _2.FL.GetTimelineItemSourceDescription(tsDesc);

                foreach (IVideoEffect _4 in _2.Effects)
                {
                    EffectDescription _5 = new(_3, _0, 0, 1, 0, 1);
                    SubChainNode _6 = _chain.FirstOrDefault(x => x.Effect == _4) ?? new(_devices, _4);
                    _6.SetInput(_1);
                    _0 = _6.Update(_5);
                    _1 = _6.Output;
                    l0.Add(_6);
                }
            }

            Output = _1;
            _chain = [.. l0];

            foreach (SubChainNode _2 in a1)
                _2.Dispose();

            return _0;
        }

        public void Dispose()
        {
            _empty.Dispose();
            
            foreach (SubChainNode _0 in _chain)
                _0.Dispose();
        }
    }
}
