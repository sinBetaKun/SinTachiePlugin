using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.LayerValueListController.Extra.Parameter
{
    public class NoExtra : LayerValueExtraBase
    {
        public NoExtra()
        {
        }

        public NoExtra(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(LayerValueExtraBase? origin)
        {
            return;
        }

        public override double GetValue(long frame, long length, int fps)
        {
            return 1.0;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void LoadSharedData(SharedDataStore store)
        {
        }

        protected override void SaveSharedData(SharedDataStore store)
        {
        }
    }
}
