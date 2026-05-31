namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.StringInput
{
    internal class StringInputSharedData
    {
        public string StringInput { get; set; } = string.Empty;

        public StringInputSharedData()
        {
        }

        public StringInputSharedData(IStringInputParameter parameter)
        {
            StringInput = parameter.StringInput;
        }

        public void CopyTo(IStringInputParameter parameter)
        {
            parameter.StringInput = StringInput;
        }
    }
}
