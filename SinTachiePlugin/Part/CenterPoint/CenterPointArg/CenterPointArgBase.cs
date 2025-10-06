using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg
{
    internal abstract class CenterPointArgBase : SharedParameterBase
    {
        public CenterPointArgBase()
        {
        }

        public CenterPointArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract void CopyFrom(CenterPointArgBase? origin);
    }
}
