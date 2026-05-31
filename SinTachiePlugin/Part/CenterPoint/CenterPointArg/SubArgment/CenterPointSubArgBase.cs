using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument
{
    internal abstract class CenterPointSubArgBase : SharedParameterBase
    {
        public CenterPointSubArgBase()
        {
        }

        public CenterPointSubArgBase(SharedDataStore? store = null) : base(store)
        {   
        }

        public abstract CenterPointSubArgBase GetClone();

        public abstract void CopyFrom(CenterPointSubArgBase? origin);
    }
}
