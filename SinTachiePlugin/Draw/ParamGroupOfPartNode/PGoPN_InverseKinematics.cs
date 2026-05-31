using SinTachiePlugin.Part;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.Parameter;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Parameter;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Draw.ParamGroupOfPartNode
{
    internal class PGoPN_InverseKinematics
    {
        public string RootPartTag { get; private set; } = string.Empty;
        public string JointPartTag { get; private set; } = string.Empty;
        public string EndEffectorPartTag { get; private set; } = string.Empty;
        public string EndEffectorPointName { get; private set; } = string.Empty;
        public double EndEffectorPointX { get; private set; }
        public double EndEffectorPointY { get; private set; }
        public string PreEndEffectorPointName { get; private set; } = string.Empty;
        public double PreEndEffectorPointX { get; private set; }
        public double PreEndEffectorPointY { get; private set; }

        public void Reset()
        {
            RootPartTag = JointPartTag = EndEffectorPartTag = EndEffectorPointName = PreEndEffectorPointName = string.Empty;
            EndEffectorPointX = EndEffectorPointY = PreEndEffectorPointX = PreEndEffectorPointY = 0;
        }

        public void Update(List<PartValueAndFL> pvfls)
        {
            foreach (PartValueAndFL pvfl in pvfls.Reverse<PartValueAndFL>())
            {
                ControlledParametersOfPart cpp = pvfl.PartValue.ControlledParameters;

                if (cpp.InverseKinematicsArg is HavingSourceInverseKinematicsParameter hsikp)
                {
                    if (hsikp.SubArg is NoneIKParameter)
                    {
                        break;
                    }
                    else if (hsikp.SubArg is TypeAIKParameter tap)
                    {
                        RootPartTag = tap.RootPartTag;
                        JointPartTag = tap.JointPartTag;
                        EndEffectorPartTag = tap.EndEffectorPartTag;
                        EndEffectorPointName = tap.EndEffectorPointName;
                        EndEffectorPointX = 0;
                        EndEffectorPointY = 0;
                        PreEndEffectorPointName = tap.PreEndEffectorPointName;
                        PreEndEffectorPointX = 0;
                        PreEndEffectorPointY = 0;

                        return;
                    }
                }
            }

            RootPartTag = string.Empty;
            JointPartTag = string.Empty;
            EndEffectorPartTag = string.Empty;
            EndEffectorPointName = string.Empty;
            EndEffectorPointX = 0;
            EndEffectorPointY = 0;
            PreEndEffectorPointName = string.Empty;
            PreEndEffectorPointX = 0;
            PreEndEffectorPointY = 0;
        }

        public void CopyTo(PGoPN_InverseKinematics pg)
        {
            pg.RootPartTag = RootPartTag;
            pg.JointPartTag = JointPartTag;
            pg.EndEffectorPartTag = EndEffectorPartTag;
            pg.EndEffectorPointName = EndEffectorPointName;
            pg.EndEffectorPointX = EndEffectorPointX;
            pg.EndEffectorPointY = EndEffectorPointY;
            pg.PreEndEffectorPointName = PreEndEffectorPointName;
            pg.PreEndEffectorPointX = PreEndEffectorPointX;
            pg.PreEndEffectorPointY = PreEndEffectorPointY;
        }

        public PartNodeComparateResult Comparate(PGoPN_InverseKinematics pg)
        {
            bool networkChanged =
                pg.RootPartTag != RootPartTag ||
                pg.JointPartTag != JointPartTag ||
                pg.EndEffectorPartTag != EndEffectorPartTag;

            bool valueChanged =
                pg.EndEffectorPointName != EndEffectorPointName ||
                pg.EndEffectorPointX != EndEffectorPointX ||
                pg.EndEffectorPointY != EndEffectorPointY ||
                pg.PreEndEffectorPointName != PreEndEffectorPointName ||
                pg.PreEndEffectorPointX != PreEndEffectorPointX ||
                pg.PreEndEffectorPointY != PreEndEffectorPointY;

            return new(PartNetwork: networkChanged, Value: valueChanged);
        }
    }
}
