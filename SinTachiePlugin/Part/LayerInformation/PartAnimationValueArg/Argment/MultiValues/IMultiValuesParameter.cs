using SinTachiePlugin.PartAnimation;
using System.Collections.Immutable;

namespace SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Argment.MultiValues
{
    internal interface IMultiValuesParameter
    {
        public ImmutableList<PartAnimationValueExtra> PartAnimationValues { get; set; }
    }
}
