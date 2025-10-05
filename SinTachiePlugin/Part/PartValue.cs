using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Part
{
    internal class PartValue : Animatable
    {
        [Display(AutoGenerateField = true)]
        public ControledParametersOfPart LayerInformationGroup { get => layerInformationGroup; set => Set(ref layerInformationGroup, value); }
        private ControledParametersOfPart layerInformationGroup = new();

        protected override IEnumerable<IAnimatable> GetAnimatables() => [LayerInformationGroup];
    }
}
