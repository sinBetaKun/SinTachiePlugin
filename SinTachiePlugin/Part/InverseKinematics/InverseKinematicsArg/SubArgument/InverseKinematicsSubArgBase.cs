using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument
{
    internal abstract class InverseKinematicsSubArgBase : SharedParameterBase
    {
        public InverseKinematicsSubArgBase()
        {
        }

        public InverseKinematicsSubArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract InverseKinematicsSubArgBase GetClone();

        public abstract void CopyFrom(InverseKinematicsSubArgBase? origin);
    }
}
