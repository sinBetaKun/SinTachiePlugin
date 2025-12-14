using SinTachiePlugin.Parts;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Part
{
    public class PartValue : Animatable
    {
        [JsonIgnore]
        public int Depth { get => _depth; set => Set(ref _depth, value); }
        private int _depth = 0;

        public int ParentIndex { get => _parentIndex; set => Set(ref _parentIndex, value); }
        private int _parentIndex = -1;

        [Display(AutoGenerateField = true)]
        public ControlledParametersOfPart ControlledParameters { get => controledParameters; set => Set(ref controledParameters, value); }
        private ControlledParametersOfPart controledParameters = new();

        protected override IEnumerable<IAnimatable> GetAnimatables() => [ControlledParameters];

        public PartValue()
        {
        }

        public PartValue(PartValue origin)
        {
            ParentIndex = origin.ParentIndex;
            ControlledParameters = new (origin.ControlledParameters);
        }

        [Obsolete]
        public PartValue(PartBlock block)
        {
            Depth = 0;
            ParentIndex = -1;
            ControlledParameters = new(block);
        }
    }
}
