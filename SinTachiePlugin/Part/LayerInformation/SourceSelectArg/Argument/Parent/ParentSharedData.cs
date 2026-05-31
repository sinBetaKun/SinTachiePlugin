namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Parent
{
    internal class ParentSharedData
    {
        public string Parent { get; set; } = string.Empty;

        public ParentSharedData()
        {
        }

        public ParentSharedData(IParentParameter parameter)
        {
            Parent = parameter.Parent;
        }

        public void CopyTo(IParentParameter parameter)
        {
            parameter.Parent = Parent;
        }
    }
}
