using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.PreEndEffectorPointXY
{
    internal class PreEndEffectorPointXYSharedData
    {
        public Animation PreEndEffectorPointX { get; } = new(0, -10000, 10000);
        public Animation PreEndEffectorPointY { get; } = new(0, -10000, 10000);

        public PreEndEffectorPointXYSharedData()
        {
        }

        public PreEndEffectorPointXYSharedData(IPreEndEffectorPointXYParameter parameter)
        {
            PreEndEffectorPointX.CopyFrom(parameter.PreEndEffectorPointX);
            PreEndEffectorPointY.CopyFrom(parameter.PreEndEffectorPointY);
        }

        public void CopyTo(IPreEndEffectorPointXYParameter parameter)
        {
            parameter.PreEndEffectorPointX.CopyFrom(PreEndEffectorPointX);
            parameter.PreEndEffectorPointY.CopyFrom(PreEndEffectorPointY);
        }
    }
}
