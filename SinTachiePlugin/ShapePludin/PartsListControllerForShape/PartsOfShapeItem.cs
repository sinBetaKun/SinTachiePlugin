using SinTachiePlugin.Parts;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.ShapePludin.PartsListControllerForShape
{
    public class PartsOfShapeItem : Animatable, IFileItem
    {
        public string Root { get; set => Set(ref field, value); } = string.Empty;

        public ImmutableList<PartBlock> Parts{ get; set => Set(ref field, value); } = [];

        protected override IEnumerable<IAnimatable> GetAnimatables() => [.. Parts];

        public IEnumerable<string> GetFiles() => [Root, .. Parts.SelectMany(x => x.GetFiles())];

        public void ReplaceFile(string from, string to)
        {
            if (Root == from)
            {
                Root = to;
                return;
            }

            Parts.ForEach(x => x.ReplaceFile(from, to));
        }

        public IEnumerable<TimelineResource> GetResources()
        {
            if (TimelineResource.TryParseFromPath(Root, TimelineResourceType.Tachie, out TimelineResource? tr_0))
                yield return tr_0;

            foreach (PartBlock part in Parts)
                foreach (TimelineResource tr_1 in part.GetResources())
                    yield return tr_1;
        }
    }
}
