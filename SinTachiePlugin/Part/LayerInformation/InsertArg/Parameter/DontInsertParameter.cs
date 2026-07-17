using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.InsertArg.Parameter
{
    internal class DontInsertParameter : InsertArgBase
    {
        public DontInsertParameter()
        {
        }

        public DontInsertParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(InsertArgBase? origin)
        {
        }

        public override InsertArgBase GetClone()
        {
            DontInsertParameter clone = new();
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
