using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.PartEffect.PartEffectArg
{
    public abstract class PartEffectArgBase : SharedParameterBase
    {
        public PartEffectArgBase()
        {
        }

        public PartEffectArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract void CopyFrom(PartEffectArgBase? origin);
    }
}
