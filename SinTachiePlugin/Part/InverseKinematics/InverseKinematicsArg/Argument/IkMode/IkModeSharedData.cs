using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.Argument.IkMode
{
    internal class IkModeSharedData
    {
        public IKMode IkMode { get; set; } = IKMode.None;

        public IkModeSharedData()
        {
        }

        public IkModeSharedData(IIkModeParameter parameter)
        {
            IkMode = parameter.IkMode;
        }

        public void CopyTo(IIkModeParameter parameter)
        {
            parameter.IkMode = IkMode;
        }
    }
}
