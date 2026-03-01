using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg
{
    internal static class LayerTypeEx
    {
        public static SourceSelectArgBase Convert(this LayerType type, SourceSelectArgBase current)
        {
            var store = current.GetSharedData();
            SourceSelectArgBase param = type switch
            {
                LayerType.Image => new ImageFileParameter(store),
                LayerType.Psd => new PsdFileParameter(store),
                LayerType.Video => new VideoFileParameter(store),
                LayerType.Scene => new SceneParameter(store),
                LayerType.Group => new GroupParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
