using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.PartAnimationValue
{
    internal interface IPartAnimationValueParameter
    {
        public PartAnimationValueMode PartAnimationValueMode { get; set; }
        public PartAnimationValueArgBase PartAnimationValueArg { get; set; }
    }
}