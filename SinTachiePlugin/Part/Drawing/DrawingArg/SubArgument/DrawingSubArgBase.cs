using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument
{
    internal abstract class DrawingSubArgBase : SharedParameterBase
    {
        public DrawingSubArgBase()
        {
        }

        public DrawingSubArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract DrawingSubArgBase GetClone();

        public abstract void CopyFrom(DrawingSubArgBase? origin);
    }
}
