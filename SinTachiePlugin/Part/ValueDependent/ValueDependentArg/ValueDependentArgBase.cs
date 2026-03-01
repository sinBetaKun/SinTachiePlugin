using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.ValueDependent.ValueDependentArg
{
    public abstract class ValueDependentArgBase : SharedParameterBase
    {
        public ValueDependentArgBase()
        {
        }

        public ValueDependentArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract ValueDependentArgBase GetClone();

        public abstract void CopyFrom(ValueDependentArgBase? origin);
    }
}
