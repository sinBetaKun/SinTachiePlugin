namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.JointPartTag
{
    internal class JointPartTagSharedData
    {
        public string JointPartTag { get; set; } = string.Empty;

        public JointPartTagSharedData()
        {
        }

        public JointPartTagSharedData(IJointPartTagParameter parameter)
        {
            JointPartTag = parameter.JointPartTag;
        }

        public void CopyTo(IJointPartTagParameter parameter)
        {
            parameter.JointPartTag = JointPartTag;
        }
    }
}
