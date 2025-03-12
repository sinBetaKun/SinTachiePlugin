using SinTachiePlugin.Enums;
using SinTachiePlugin.Parts;
using SinTachiePlugin.TachiePlugin;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.ShapePludin
{
    internal class ShapeOfSinTachieSource : SinTachieSourceBase, IShapeSource
    {
        readonly ShapeParameterOfSinTachie param;

        public ID2D1Image Output => commandList ?? throw new InvalidOperationException("commandList is null");

        public ShapeOfSinTachieSource(IGraphicsDevicesAndContext devices, ShapeParameterOfSinTachie param) : base(devices)
        {
            this.param = param;
        }

        public void Update(TimelineItemSourceDescription description)
        {
            UpdateCase updateCase = UpdateNodeListForShape(description);

            if (updateCase != UpdateCase.None)
            {
                if (updateCase.HasFlag(UpdateCase.BitmapParams))
                {
                    UpdateParentPaths();
                    UpdateOutputs(description);
                }
                SetCommandList(updateCase);
            }
        }

        private UpdateCase UpdateNodeListForShape(TimelineItemSourceDescription description)
        {
            FrameAndLength fl = new(description);
            return UpdateNodeList(
                [.. param.PartsAndRoot.Parts],
                [.. Enumerable.Repeat(fl, param.PartsAndRoot.Parts.Count)],
                param.PartsAndRoot.Parts.Count,
                description.FPS,
                0
                );
        }
    }
}
