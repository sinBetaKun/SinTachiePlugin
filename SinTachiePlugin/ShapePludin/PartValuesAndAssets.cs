using Newtonsoft.Json;
using SinTachiePlugin.CharacterAsset;
using SinTachiePlugin.Part;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.ShapePludin
{
    internal class PartValuesAndAssets : Animatable
    {
        public ImmutableList<PartValue> PartValues { get => _partValues; set => Set(ref _partValues, value); }
        private ImmutableList<PartValue> _partValues = [];

        public ImmutableList<CharacterAssetsData> Assets
        {
            get => _assets;
            set
            {
                Set(ref _assets, value);
                AssetsChanged.Invoke(null, EventArgs.Empty);
            }
        }
        private ImmutableList<CharacterAssetsData> _assets = [];

        public event EventHandler? AssetsChanged;

        protected override IEnumerable<IAnimatable> GetAnimatables() => [.. PartValues];
    }
}
