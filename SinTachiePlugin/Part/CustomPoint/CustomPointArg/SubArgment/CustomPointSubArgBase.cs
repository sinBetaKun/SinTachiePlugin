using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgment
{
    public abstract class CustomPointSubArgBase : SharedParameterBase
    {
        public CustomPointSubArgBase()
        {
        }

        public CustomPointSubArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract CustomPointSubArgBase GetClone();

        public abstract void CopyFrom(CustomPointSubArgBase? origin);
    }
}
