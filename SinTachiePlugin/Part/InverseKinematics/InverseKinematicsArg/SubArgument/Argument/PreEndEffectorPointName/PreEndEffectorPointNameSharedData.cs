namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Argument.PreEndEffectorPointName
{
    internal class PreEndEffectorPointNameSharedData
    {
        public string PreEndEffectorPointName { get; set; } = string.Empty;

        public PreEndEffectorPointNameSharedData()
        {
        }

        public PreEndEffectorPointNameSharedData(IPreEndEffectorPointNameParameter parameter)
        {
            PreEndEffectorPointName = parameter.PreEndEffectorPointName;
        }

        public void CopyTo(IPreEndEffectorPointNameParameter parameter)
        {
            parameter.PreEndEffectorPointName = PreEndEffectorPointName;
        }
    }
}
