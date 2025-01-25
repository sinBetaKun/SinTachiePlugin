using SinTachiePlugin.Enums;
using SinTachiePlugin.Parts;
using SinTachiePlugin.TachiePlugin;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
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
            if (UpdateNodeListForShape(description))
            {
                commandList?.Dispose();

                UpdateParentPaths();

                UpdateOutputs(description);

                SetCommandList();
            }
        }

        private bool UpdateNodeListForShape(TimelineItemSourceDescription description)
        {
            FrameAndLength fl = new(description);
            int fps = description.FPS;
            List<(PartBlock block, FrameAndLength fl)> tupleList =
                param.PartsAndRoot.Parts.Select(x => (x, fl)).ToList();
            return UpdateNodeList(tupleList, fps, 0.0);
        }
    }
}
