using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg
{
    public abstract class SourceSelectArgBase : SharedParameterBase
    {
        public SourceSelectArgBase()
        {
        }

        public SourceSelectArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract SourceSelectArgBase GetClone();

        public abstract void CopyFrom(SourceSelectArgBase? origin);
    }
}
