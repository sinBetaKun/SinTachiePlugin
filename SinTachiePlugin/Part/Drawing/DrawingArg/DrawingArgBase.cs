using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Drawing.DrawingArg
{
    public abstract class DrawingArgBase : SharedParameterBase
    {
        public DrawingArgBase()
        {
        }

        public DrawingArgBase(SharedDataStore? store = null) : base(store)
        {   
        }

        public abstract void CopyFrom(DrawingArgBase? origin);
    }
}
