using SinTachiePlugin.Parts;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.ShapePludin.PartsListControllerForShape
{
    public class PartsOfShapeItem : Animatable
    {
        public string Root { get => root; set => Set(ref root, value); }
        string root = string.Empty;

        public ImmutableList<PartBlock> Parts { get => parts; set => Set(ref parts, value); }
        ImmutableList<PartBlock> parts = [];

        protected override IEnumerable<IAnimatable> GetAnimatables() => [.. Parts];
    }
}
