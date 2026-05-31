using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.EndEffectorPointXY
{
    internal class EndEffectorPointXYSharedData
    {
        public Animation EndEffectorPointX { get; } = new(0, -10000, 10000);
        public Animation EndEffectorPointY { get; } = new(0, -10000, 10000);

        public EndEffectorPointXYSharedData()
        {
        }

        public EndEffectorPointXYSharedData(IEndEffectorPointXYParameter parameter)
        {
            EndEffectorPointX.CopyFrom(parameter.EndEffectorPointX);
            EndEffectorPointY.CopyFrom(parameter.EndEffectorPointY);
        }

        public void CopyTo(IEndEffectorPointXYParameter parameter)
        {
            parameter.EndEffectorPointX.CopyFrom(EndEffectorPointX);
            parameter.EndEffectorPointY.CopyFrom(EndEffectorPointY);
        }
    }
}
