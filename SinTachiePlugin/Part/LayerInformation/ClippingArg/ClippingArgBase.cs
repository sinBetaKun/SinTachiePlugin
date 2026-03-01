using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.ClippingArg
{
    internal abstract class ClippingArgBase : SharedParameterBase
    {
        public ClippingArgBase()
        {
        }

        public ClippingArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract ClippingArgBase GetClone();

        public abstract void CopyFrom(ClippingArgBase? origin);
    }
}
