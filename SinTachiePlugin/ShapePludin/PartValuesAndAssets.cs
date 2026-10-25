using SinTachiePlugin.CharacterAsset;
using SinTachiePlugin.Part;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.ShapePludin
{
    internal class PartValuesAndAssets : Animatable
    {
        public ImmutableList<PartValue> PartValues { get; set => Set(ref field, value); } = [];

        public ImmutableList<CharacterAssetsData> Assets
        {
            get;
            set
            {
                Set(ref field, value);
                AssetsChanged?.Invoke(null, EventArgs.Empty);
            }
        } = [];

        public event EventHandler? AssetsChanged;

        protected override IEnumerable<IAnimatable> GetAnimatables() => [.. PartValues];
    }
}
