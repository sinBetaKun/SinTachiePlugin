using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.Parameter
{
    internal class WithoutSourceInverseKinematicsParameter : InverseKinematicsArgBase
    {
        public WithoutSourceInverseKinematicsParameter()
        {
        }

        public WithoutSourceInverseKinematicsParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(InverseKinematicsArgBase? origin)
        {
        }

        public override InverseKinematicsArgBase GetClone()
        {
            WithoutSourceInverseKinematicsParameter clone = new();
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
