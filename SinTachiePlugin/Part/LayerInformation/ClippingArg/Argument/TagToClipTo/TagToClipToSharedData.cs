namespace SinTachiePlugin.Part.LayerInformation.ClippingArg.Argument.TagToClipTo
{
    internal class TagToClipToSharedData
    {
        public string TagToClipTo { get; set; } = string.Empty;

        public TagToClipToSharedData()
        {
        }

        public TagToClipToSharedData(ITagToClipToParameter parameter)
        {
            TagToClipTo = parameter.TagToClipTo;
        }

        public void CopyTo(ITagToClipToParameter parameter)
        {
            parameter.TagToClipTo = TagToClipTo;
        }
    }
}
