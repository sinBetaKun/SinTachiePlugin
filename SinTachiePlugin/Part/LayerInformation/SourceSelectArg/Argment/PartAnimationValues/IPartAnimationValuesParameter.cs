using System.Collections.Immutable;
using SinTachiePlugin.PartAnimation;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.PartAnimationValues
{
    internal interface IPartAnimationValuesParameter
    {
        public ImmutableList<PartAnimationValue> PartAnimationValues { get; set; }
    }
}
