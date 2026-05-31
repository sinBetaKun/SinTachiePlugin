using SinTachiePlugin.PartAnimation;
using System.Collections.Immutable;

namespace SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Argument.MultiValues
{
    internal class MultiValuesSharedData
    {
        public ImmutableList<PartAnimationValueExtra> PartAnimationValues { get; set; } = [];

        public MultiValuesSharedData()
        {
        }

        public MultiValuesSharedData(IMultiValuesParameter parameter)
        {
            PartAnimationValues = [.. parameter.PartAnimationValues.Select(pav => new PartAnimationValueExtra(pav))];
        }

        public void CopyTo(IMultiValuesParameter parameter)
        {
            parameter.PartAnimationValues = [.. PartAnimationValues.Select(pav => new PartAnimationValueExtra(pav))];
        }
    }
}
