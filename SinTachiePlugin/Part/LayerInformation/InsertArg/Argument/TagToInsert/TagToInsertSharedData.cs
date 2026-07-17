namespace SinTachiePlugin.Part.LayerInformation.InsertArg.Argument.TagToInsert
{
    internal class TagToInsertSharedData
    {
        public string TagToInsert { get; set; } = string.Empty;

        public TagToInsertSharedData()
        {
        }

        public TagToInsertSharedData(ITagToInsertParameter parameter)
        {
            TagToInsert = parameter.TagToInsert;
        }

        public void CopyTo(ITagToInsertParameter parameter)
        {
            parameter.TagToInsert = TagToInsert;
        }
    }
}
