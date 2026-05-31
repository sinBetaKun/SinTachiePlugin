using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Parameter;

namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.Argument.SubArg
{
    internal class SubArgSharedData
    {
        public InverseKinematicsSubArgBase SubArg { get; set; } = new NoneIKParameter();

        public SubArgSharedData()
        {
        }

        public SubArgSharedData(ISubArgParameter parameter)
        {
            SubArg = parameter.SubArg.GetClone();
        }

        public void CopyTo(ISubArgParameter parameter)
        {
            parameter.SubArg = SubArg.GetClone();
        }
    }
}
