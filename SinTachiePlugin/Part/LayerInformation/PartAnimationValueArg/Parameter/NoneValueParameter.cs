using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Parameter
{
    internal class NoneValueParameter : PartAnimationValueArgBase
    {
        public NoneValueParameter()
        {
        }

        public NoneValueParameter(SharedDataStore? store) : base(store)
        {   
        }

        public override void CopyFrom(PartAnimationValueArgBase? origin)
        {
        }

        public override PartAnimationValueArgBase GetClone()
        {
            NoneValueParameter clone = new();
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
