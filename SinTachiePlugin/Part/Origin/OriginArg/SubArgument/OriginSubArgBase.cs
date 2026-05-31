using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Origin.OriginArg.SubArgument
{
    internal abstract class OriginSubArgBase : SharedParameterBase
    {
        public OriginSubArgBase()
        {
        }

        public OriginSubArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract OriginSubArgBase GetClone();

        public abstract void CopyFrom(OriginSubArgBase? origin);
    }
}
