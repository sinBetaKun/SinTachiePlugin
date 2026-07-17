using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.InsertArg
{
    internal abstract class InsertArgBase : SharedParameterBase
    {
        public InsertArgBase()
        {
        }

        public InsertArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract InsertArgBase GetClone();

        public abstract void CopyFrom(InsertArgBase? origin);
    }
}
