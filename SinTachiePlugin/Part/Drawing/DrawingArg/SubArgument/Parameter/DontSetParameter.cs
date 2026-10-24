using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument.Parameter
{
    internal class DontSetParameter : DrawingSubArgBase
    {
        public DontSetParameter()
        {
        }

        public DontSetParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(DrawingSubArgBase? origin)
        {
        }

        public override DrawingSubArgBase GetClone()
        {
            DontSetParameter clone = new();
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
