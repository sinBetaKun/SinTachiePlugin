namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.ImageFilePath
{
    internal class ImageFilePathSharedData
    {
        public string ImageFilePath { get; set; } = string.Empty;

        public ImageFilePathSharedData()
        {
        }

        public ImageFilePathSharedData(IImageFilePathParameter parameter)
        {
            ImageFilePath = parameter.ImageFilePath;
        }

        public void CopyTo(IImageFilePathParameter parameter)
        {
            parameter.ImageFilePath = ImageFilePath;
        }
    }
}
