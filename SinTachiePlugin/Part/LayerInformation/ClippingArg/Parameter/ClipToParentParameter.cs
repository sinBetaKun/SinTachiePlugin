using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.ClippingArg.Parameter
{
    internal class ClipToParentParameter : ClippingArgBase
    {
        public ClipToParentParameter()
        {
        }

        public ClipToParentParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(ClippingArgBase? origin)
        {
        }

        public override ClippingArgBase GetClone()
        {
            ClipToParentParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
        }
    }
}
