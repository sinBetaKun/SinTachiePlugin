namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.EndEffectorPointName
{
    internal class EndEffectorPointNameSharedData
    {
        public string EndEffectorPointName { get; set; } = string.Empty;

        public EndEffectorPointNameSharedData()
        {
        }

        public EndEffectorPointNameSharedData(IEndEffectorPointNameParameter parameter)
        {
            EndEffectorPointName = parameter.EndEffectorPointName;
        }

        public void CopyTo(IEndEffectorPointNameParameter parameter)
        {
            parameter.EndEffectorPointName = EndEffectorPointName;
        }
    }
}
