using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg
{
    public abstract class InverseKinematicsArgBase : SharedParameterBase
    {
        public InverseKinematicsArgBase()
        {
        }

        public InverseKinematicsArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract InverseKinematicsArgBase GetClone();
        public abstract void CopyFrom(InverseKinematicsArgBase? origin);
    }
}
