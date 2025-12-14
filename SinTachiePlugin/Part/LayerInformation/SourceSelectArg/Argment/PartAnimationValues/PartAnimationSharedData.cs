using System.Collections.Immutable;
using SinTachiePlugin.PartAnimation;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.PartAnimationValues
{
    internal class PartAnimationSharedData
    {
        public ImmutableList<PartAnimationValue> PartAnimationValues { get; set; } = [];

        public PartAnimationSharedData()
        {
        }

        public PartAnimationSharedData(IPartAnimationValuesParameter parameter)
        {
            PartAnimationValues = [.. parameter.PartAnimationValues];
        }

        public void CopyTo(IPartAnimationValuesParameter parameter)
        {
            parameter.PartAnimationValues = [.. PartAnimationValues];
        }
    }
}
