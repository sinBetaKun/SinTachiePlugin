using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg.Parameter;

namespace SinTachiePlugin.Part.InverseKinematics.InverseKinematicsArg
{
    internal static class LayerTypeEx
    {
        public static InverseKinematicsArgBase Convert(this LayerType type, InverseKinematicsArgBase current)
        {
            var store = current.GetSharedData();
            InverseKinematicsArgBase param = type switch
            {
                LayerType.Image => new HavingSourceInverseKinematicsParameter(store),
                LayerType.Psd => new HavingSourceInverseKinematicsParameter(store),
                LayerType.Video => new HavingSourceInverseKinematicsParameter(store),
                LayerType.Scene => new HavingSourceInverseKinematicsParameter(store),
                LayerType.Group => new WithoutSourceInverseKinematicsParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
