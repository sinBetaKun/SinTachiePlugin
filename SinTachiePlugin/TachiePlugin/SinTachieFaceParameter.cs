using SinTachiePlugin.Parts.Controller;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin.Tachie;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Parts
{
    internal class SinTachieFaceParameter : TachieFaceParameterBase, IFileItem
    {
        [Display]
        [PartsListController(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public ImmutableList<PartBlock> Parts { get => parts; set => Set(ref parts, value); }
        ImmutableList<PartBlock> parts = [];

        /// <summary>
        /// クラス内のIAnimatableを列挙する。
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IAnimatable> GetAnimatables() => Parts;

        public IEnumerable<string> GetFiles() => [.. Parts.SelectMany(x => x.GetFiles())];

        public void ReplaceFile(string from, string to)
        {
            Parts.ForEach(x => x.ReplaceFile(from, to));
        }

        public IEnumerable<TimelineResource> GetResources()
        {
            foreach (PartBlock part in Parts)
                foreach (TimelineResource tr_1 in part.GetResources())
                    yield return tr_1;
        }
    }
}
