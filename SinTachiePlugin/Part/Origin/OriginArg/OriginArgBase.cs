using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Origin.OriginArg
{
    public abstract class OriginArgBase : SharedParameterBase
    {
        public OriginArgBase()
        {
        }

        public OriginArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract OriginArgBase GetClone();

        public abstract void CopyFrom(OriginArgBase? origin);
    }
}
