using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter
{
    internal class GroupParameter : SourceSelectArgBase
    {
        public GroupParameter()
        {
        }

        public GroupParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(SourceSelectArgBase? origin)
        {
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
