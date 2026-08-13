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

        public DrawDescription Update(IEnumerable<IVideoEffect> effects, FrameAndLength fl, ID2D1Image input, TimelineSourceDescription tsDesc, DrawDescription dDesc)
        {
            Output = _empty;
            IVideoEffect[] a0 = [.. effects];
            SubChainNode[] a1 = [.. _chain.Where(x => !a0.Contains(x.Effect))];
            List<SubChainNode> l0 = [];
            TimelineItemSourceDescription _0 = fl.GetTimelineItemSourceDescription(tsDesc);
            DrawDescription _1 = dDesc;
            ID2D1Image _2 = input;

            foreach (IVideoEffect _3 in a0)
            {
                EffectDescription _4 = new(_0, _1, 0, 1, 0, 1);
                SubChainNode _5 = _chain.FirstOrDefault(x => x.Effect == _3) ?? new(_devices, _3);
                _5.SetInput(_2);
                _1 = _5.Update(_4);
                _2 = _5.Output;
                l0.Add(_5);
            }

            Output = _2;
            _chain = [.. l0];

            foreach (SubChainNode _6 in a1)
                _6.Dispose();

            return _1;
        }

        public void Dispose()
        {
            _empty.Dispose();
            
            foreach (SubChainNode _0 in _chain)
                _0.Dispose();
        }
    }
}
