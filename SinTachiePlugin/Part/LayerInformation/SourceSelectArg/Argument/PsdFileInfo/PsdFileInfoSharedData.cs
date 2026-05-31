using YukkuriMovieMaker.Plugin.Tachie.Psd;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.PsdFileInfo
{
    internal class PsdFileInfoSharedData
    {
        public PsdShapeParameter PsdFileInfo { get; set; } = new();

        public PsdFileInfoSharedData()
        {
        }

        public PsdFileInfoSharedData(IPsdFileInfoParameter parameter)
        {
            PsdFileInfo.FilePath = parameter.PsdFileInfo.FilePath;
            PsdFileInfo.EnableLayersFilePath = parameter.PsdFileInfo.EnableLayersFilePath;
            PsdFileInfo.EnableLayers = [.. parameter.PsdFileInfo.EnableLayers];
        }

        public void CopyTo(IPsdFileInfoParameter parameter)
        {
            parameter.PsdFileInfo.FilePath = PsdFileInfo.FilePath;
            parameter.PsdFileInfo.EnableLayersFilePath = PsdFileInfo.EnableLayersFilePath;
            parameter.PsdFileInfo.EnableLayers = [.. PsdFileInfo.EnableLayers];
        }
    }
}
