namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.IsOpened
{
    internal class IsOpenedSharedData
    {
        bool IsOpened { get; set; } = true;

        public IsOpenedSharedData()
        {
        }

        public IsOpenedSharedData(IIsOpenedParameter parameter)
        {
            IsOpened = parameter.IsOpened;
        }

        public void CopyTo(IIsOpenedParameter parameter)
        {
            parameter.IsOpened = IsOpened;
        }
    }
}
