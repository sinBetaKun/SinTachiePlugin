namespace SinTachiePlugin.Part.LayerInformation.ClippingArg.Argment.PartToClipTo
{
    internal class PartToClipToSharedData
    {
        public string PartToClipTo { get; set; } = string.Empty;

        public PartToClipToSharedData()
        {
        }

        public PartToClipToSharedData(IPartToClipToParameter parameter)
        {
            PartToClipTo = parameter.PartToClipTo;
        }

        public void CopyTo(IPartToClipToParameter parameter)
        {
            parameter.PartToClipTo = PartToClipTo;
        }
    }
}
