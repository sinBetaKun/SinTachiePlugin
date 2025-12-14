using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.ValueDependent.ValueDependentArg.Parameter;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.ValueDependent.ValueDependentArg
{
    internal static class LayerTypeEx
    {
        public static ValueDependentArgBase Convert(this LayerType type, ValueDependentArgBase current)
        {
            SharedDataStore store = current.GetSharedData();
            ValueDependentArgBase param = type switch
            {
                LayerType.Image => new HavingSourceValueDependentParameter(store),
                LayerType.Psd => new HavingSourceValueDependentParameter(store),
                LayerType.Video => new HavingSourceValueDependentParameter(store),
                LayerType.Scene => new HavingSourceValueDependentParameter(store),
                LayerType.Group => new WithoutSourceValueDependentParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }

        public static ValueDependentArgBase GetClone(this LayerType type, ValueDependentArgBase origin)
        {
            SharedDataStore store = origin.GetSharedData();
            ValueDependentArgBase param = type switch
            {
                LayerType.Image => new HavingSourceValueDependentParameter(store),
                LayerType.Psd => new HavingSourceValueDependentParameter(store),
                LayerType.Video => new HavingSourceValueDependentParameter(store),
                LayerType.Scene => new HavingSourceValueDependentParameter(store),
                LayerType.Group => new WithoutSourceValueDependentParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            return param;
        }
    }
}
