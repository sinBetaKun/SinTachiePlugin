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

        public override DrawingArgBase GetClone()
        {
            WithoutSourceDrawingParameter clone = new();
            clone.CopyFrom(this);
            return clone;
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
