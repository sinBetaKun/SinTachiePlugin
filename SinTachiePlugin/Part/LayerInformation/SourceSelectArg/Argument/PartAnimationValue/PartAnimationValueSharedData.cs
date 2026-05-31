using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Parameter;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.PartAnimationValue
{
    internal class PartAnimationValueSharedData
    {
        public PartAnimationValueMode PartAnimationValueMode { get; set; } = PartAnimationValueMode.None;
        public PartAnimationValueArgBase PartAnimationValueArg { get; set; } = new NoneValueParameter();

        public PartAnimationValueSharedData()
        {
        }

        public PartAnimationValueSharedData(IPartAnimationValueParameter parameter)
        {
            PartAnimationValueMode = parameter.PartAnimationValueMode;
            PartAnimationValueArg = parameter.PartAnimationValueArg.GetClone();
        }

        public void CopyTo(IPartAnimationValueParameter parameter)
        {
            parameter.PartAnimationValueMode = PartAnimationValueMode;
            parameter.PartAnimationValueArg = PartAnimationValueArg.GetClone();
        }
    }
}
