using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.Parameter
{
    internal class WithoutSourceDrawingParameter : DrawingArgBase
    {
        public WithoutSourceDrawingParameter()
        {
        }

        public WithoutSourceDrawingParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(DrawingArgBase? origin)
        {
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
        }
    }
}
