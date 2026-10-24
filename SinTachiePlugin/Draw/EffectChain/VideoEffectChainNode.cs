using System.Collections.Immutable;
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
        private ImmutableList<SubChainNode> _chain = [];

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
            PartEffectsAndFL[] pefl_a0 = [.. pefls];
            List<SubChainNode> scn_l0 = [];
            SubChainNode? scn_0 = null; // 直前でUpdateしたSubChainNode
            bool b_0 = false; // scn_0がUpdate時にOutputが変化したか
            ID2D1Image img_0 = input;
            DrawDescription dd_0 = dDesc;

            foreach (PartEffectsAndFL pefl_0 in pefls)
            {
                TimelineItemSourceDescription dsc_0 = pefl_0.FL.GetTimelineItemSourceDescription(tsDesc);

                foreach (IVideoEffect ve_0 in pefl_0.Effects)
                {
                    int i_0 = _chain.FindIndex(x => x.Effect == ve_0);
                    SubChainNode scn_1;
                    bool b_1;

                    if (i_0 < 0)
                    {
                        scn_1 = new(_devices, ve_0);
                        b_1 = true;
                    }
                    else
                    {
                        scn_1 = _chain[i_0];

                        if (b_0)
                            b_1 = true;
                        else if(i_0 == 0)
                            b_1 = scn_0 is null;
                        else
                            b_1 = scn_0 == _chain[i_0 - 1];
                    }

                    if (b_1)
                        scn_1.SetInput(img_0);

                    EffectDescription ed_0 = new(dsc_0, dd_0, 0, 1, 0, 1);
                    ID2D1Image img_1 = scn_1.Output;
                    dd_0 = scn_1.Update(ed_0);
                    img_0 = scn_1.Output;
                    scn_l0.Add(scn_1);
                    scn_0 = scn_1;
                    b_0 = img_0 != img_1;
                }
            }

            foreach (SubChainNode scn_1 in _chain.Except(scn_l0))
                scn_1.Dispose();

            Output = img_0;
            _chain = [.. scn_l0];

            return dd_0;
        }

        public void Dispose()
        {
            _empty.Dispose();
            
            foreach (SubChainNode scn_0 in _chain)
                scn_0.Dispose();
        }
    }
}
