using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.ValueDependent.ValueDependentArg.SubArgument
{
    internal abstract class ValueDependentSubArgBase : SharedParameterBase
    {
        public ValueDependentSubArgBase()
        {
        }

        public ValueDependentSubArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract ValueDependentSubArgBase GetClone();

        public abstract void CopyFrom(ValueDependentSubArgBase origin);
    }
}
