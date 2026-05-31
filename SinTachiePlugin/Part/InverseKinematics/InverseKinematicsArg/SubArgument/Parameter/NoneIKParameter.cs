using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Parameter
{
    internal class NoneIKParameter : InverseKinematicsSubArgBase
    {
        public NoneIKParameter()
        {
        }

        public NoneIKParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(InverseKinematicsSubArgBase? origin)
        {
        }

        public override InverseKinematicsSubArgBase GetClone()
        {
            NoneIKParameter clone = new();
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
