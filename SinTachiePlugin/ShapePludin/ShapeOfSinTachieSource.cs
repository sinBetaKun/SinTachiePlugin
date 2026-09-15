using SinTachiePlugin.Draw;
using SinTachiePlugin.Draw.MultiPartBlend;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.ShapePludin
{
    internal class ShapeOfSinTachieSource(IGraphicsDevicesAndContext devices, ShapeParameterOfSinTachie param) : IShapeSource
    {
        private readonly ShapeParameterOfSinTachie _param = param;

        private readonly PartNodeListManager _pnlm = new(devices);
        public ID2D1Image Output => _pnlm.Output;

        public void Update(TimelineItemSourceDescription desc)
        {
            FrameAndLength fl_0 = new(desc);
            PartNodeComparateResult cmp_0 = _pnlm.UpdateOverallParameter(
                _param.PartValuesAndAssets.PartValues.Select(x => new PartValueAndFL(x, fl_0))
                , desc);

            if (cmp_0.Value)
                _pnlm.UpdateOutput(desc, cmp_0);
        }

        public void Dispose()
        {
            _pnlm.Dispose();
        }
    }
}
