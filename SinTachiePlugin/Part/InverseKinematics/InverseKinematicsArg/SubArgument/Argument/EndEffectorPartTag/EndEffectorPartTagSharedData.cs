namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.EndEffectorPartTag
{
    internal class EndEffectorPartTagSharedData
    {
        public string EndEffectorPartTag { get; set; } = string.Empty;

        public EndEffectorPartTagSharedData()
        {
        }

        public EndEffectorPartTagSharedData(IEndEffectorPartTagParameter parameter)
        {
            EndEffectorPartTag = parameter.EndEffectorPartTag;
        }

        public void CopyTo(IEndEffectorPartTagParameter parameter)
        {
            parameter.EndEffectorPartTag = EndEffectorPartTag;
        }
    }
}
