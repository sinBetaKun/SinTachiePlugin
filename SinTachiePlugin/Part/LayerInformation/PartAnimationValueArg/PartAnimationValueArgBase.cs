using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg
{
    internal abstract class PartAnimationValueArgBase : SharedParameterBase
    {
        public PartAnimationValueArgBase()
        {
        }

        public PartAnimationValueArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract PartAnimationValueArgBase GetClone();

        public abstract void CopyFrom(PartAnimationValueArgBase? origin);
    }
}
