namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.RootPartTag
{
    internal class RootPartTagSharedData
    {
        public string RootPartTag { get; set; } = string.Empty;

        public RootPartTagSharedData()
        {
        }

        public RootPartTagSharedData(IRootPartTagParameter parameter)
        {
            RootPartTag = parameter.RootPartTag; 
        }

        public void CopyTo(IRootPartTagParameter parameter)
        {
            parameter.RootPartTag = RootPartTag;
        }
    }
}
