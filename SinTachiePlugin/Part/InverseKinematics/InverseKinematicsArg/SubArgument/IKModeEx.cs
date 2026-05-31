using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument.Parameter;

namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.SubArgument
{
    internal static class IKModeEx
    {
        public static InverseKinematicsSubArgBase Convert(this IKMode mode, InverseKinematicsSubArgBase current)
        {
            var store = current.GetSharedData();
            InverseKinematicsSubArgBase param = mode switch
            {
                IKMode.None => new NoneIKParameter(store),
                IKMode.IKMode_TypeA => new TypeAIKParameter(store),
                IKMode.DontOverride => new DontOverrideIKParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
