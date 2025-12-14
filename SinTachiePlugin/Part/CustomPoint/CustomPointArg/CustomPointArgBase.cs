using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg
{
    public abstract class CustomPointArgBase : SharedParameterBase
    {
        public CustomPointArgBase()
        {
        }

        public CustomPointArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract void CopyFrom(CustomPointArgBase? origin);
    }
}
