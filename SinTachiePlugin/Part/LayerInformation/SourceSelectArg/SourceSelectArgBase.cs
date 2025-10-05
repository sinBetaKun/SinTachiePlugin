using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg
{
    internal abstract class SourceSelectArgBase : SharedParameterBase
    {
        public SourceSelectArgBase()
        {
        }

        public SourceSelectArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract void CopyFrom(SourceSelectArgBase? origin);
    }
}
