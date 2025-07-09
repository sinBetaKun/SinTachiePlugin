using SinTachiePlugin.Parts;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.LayerValueListController.Extra
{
    public abstract class LayerValueExtraBase : SharedParameterBase
    {
        public LayerValueExtraBase()
        {
        }

        public LayerValueExtraBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract double GetValue(FrameAndLength fl, int fps);
        public abstract void CopyFrom(LayerValueExtraBase? origin);
    }
}
