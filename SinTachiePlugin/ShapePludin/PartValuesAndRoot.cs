using Newtonsoft.Json;
using SinTachiePlugin.Part;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.ShapePludin
{
    internal class PartValuesAndRoot : Animatable
    {
        public ImmutableList<PartValue> PartValues { get => _partValues; set => Set(ref _partValues, value); }
        private ImmutableList<PartValue> _partValues = [];

        [JsonIgnore]
        public string Root
        {
            get => _root;
            set
            {
                RootChanged.Invoke(null, EventArgs.Empty);
                Set(ref _root, value);
            }
        }
        private string _root = string.Empty;

        public event EventHandler? RootChanged;

        protected override IEnumerable<IAnimatable> GetAnimatables() => [.. PartValues];
    }
}
