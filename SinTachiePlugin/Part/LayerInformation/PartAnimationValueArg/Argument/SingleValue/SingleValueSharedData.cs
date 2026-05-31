using SinTachiePlugin.PartAnimation;

namespace SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Argument.SingleValue
{
    internal class SingleValueSharedData
    {
        public PartAnimationValue PartAnimationValue { get; set; } = new();

        public SingleValueSharedData()
        {
        }

        public SingleValueSharedData(ISingleValueParameter parameter)
        {
            PartAnimationValue = new(parameter.PartAnimationValue);
        }

        public void CopyTo(ISingleValueParameter parameter)
        {
            parameter.PartAnimationValue = new(PartAnimationValue);
        }
    }
}
