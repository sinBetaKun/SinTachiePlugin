using SinTachiePlugin.Draw;
using SinTachiePlugin.Draw.OverallManaging;
using SinTachiePlugin.Parts;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Tachie;
using FrameAndLength = SinTachiePlugin.Draw.FrameAndLength;

namespace SinTachiePlugin.TachiePlugin
{
    internal class SinTachieSource(IGraphicsDevicesAndContext devices) : ITachieSource2
    {
        private readonly PartNodeListManager _pnlm = new(devices);
        public ID2D1Image Output => _pnlm.Output;

        /// <summary>
        /// 表示を更新する
        /// </summary>
        public void Update(TachieSourceDescription desc)
        {
            if (desc.Tachie.ItemParameter is not SinTachieItemParameter ip_0)
            {
                _pnlm.Reset();
                return;
            }

            TachieFaceDescription[] tfd_a0 = [.. desc.Tachie.Faces];
            IEnumerable<TachieFaceDescription> tfd_e0 = tfd_a0.Where(x => x.Layer < desc.Layer).OrderBy(x => x.Layer);
            IEnumerable<TachieFaceDescription> tfd_e1 = tfd_a0.Where(x => x.Layer > desc.Layer).OrderBy(x => x.Layer);
            List<PartValueAndFL> pvfl_l0 = [];

            foreach (TachieFaceDescription tfd_0 in tfd_e0)
            {
                if (tfd_0.FaceParameter is not SinTachieFaceParameter fp_0)
                    continue;
                
                FrameAndLength fl_0 = new(tfd_0.ItemPosition.Frame, tfd_0.ItemDuration.Frame);
                pvfl_l0.AddRange(fp_0.PartValues.Select(x => new PartValueAndFL(x, fl_0)));
            }

            FrameAndLength fl_1 = new(desc.ItemPosition.Frame, desc.ItemDuration.Frame);
            pvfl_l0.AddRange(ip_0.PartValues.Select(x => new PartValueAndFL(x, fl_1)));

            foreach (TachieFaceDescription tfd_0 in tfd_e1)
            {
                if (tfd_0.FaceParameter is not SinTachieFaceParameter fp_0)
                    continue;

                FrameAndLength fl = new(tfd_0.ItemPosition.Frame, tfd_0.ItemDuration.Frame);
                pvfl_l0.AddRange(fp_0.PartValues.Select(x => new PartValueAndFL(x, fl)));
            }

            if (pvfl_l0.Count == 0)
            {
                _pnlm.Reset();
                return;
            }

            _pnlm.Update(pvfl_l0, desc);
        }

        public void Dispose()
        {
            _pnlm.Dispose();
        }
    }
}
