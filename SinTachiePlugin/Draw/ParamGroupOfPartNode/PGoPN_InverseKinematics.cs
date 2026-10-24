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
            foreach (PartValueAndFL pvfl_0 in pvfls.Reverse<PartValueAndFL>())
            {
                ControlledParametersOfPart cpp_0 = pvfl_0.PartValue.ControlledParameters;

                if (cpp_0.InverseKinematicsArg is HavingSourceInverseKinematicsParameter hsikp_0)
                {
                    if (hsikp_0.SubArg is NoneIKParameter)
                    {
                        break;
                    }
                    else if (hsikp_0.SubArg is TypeAIKParameter tap_0)
                    {
                        RootPartTag = tap_0.RootPartTag;
                        JointPartTag = tap_0.JointPartTag;
                        EndEffectorPartTag = tap_0.EndEffectorPartTag;
                        EndEffectorPointName = tap_0.EndEffectorPointName;
                        EndEffectorPointX = 0;
                        EndEffectorPointY = 0;
                        PreEndEffectorPointName = tap_0.PreEndEffectorPointName;
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
            // 描画位置の変化として扱う方が楽なのでValueのみ更新
            bool b_4v =
                pg.RootPartTag != RootPartTag ||
                pg.JointPartTag != JointPartTag ||
                pg.EndEffectorPartTag != EndEffectorPartTag ||
                pg.EndEffectorPointName != EndEffectorPointName ||
                pg.EndEffectorPointX != EndEffectorPointX ||
                pg.EndEffectorPointY != EndEffectorPointY ||
                pg.PreEndEffectorPointName != PreEndEffectorPointName ||
                pg.PreEndEffectorPointX != PreEndEffectorPointX ||
                pg.PreEndEffectorPointY != PreEndEffectorPointY;

            return new(Value: b_4v);
        }
    }
}
