using SinTachiePlugin.Control.PartValueList;
using SinTachiePlugin.Part;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin.Tachie;

namespace SinTachiePlugin.Parts
{
    internal class SinTachieFaceParameter : TachieFaceParameterBase
    {
        [Display]
        [PartValueList(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public ImmutableList<PartValue> PartValues { get => _partValues; set => Set(ref _partValues, value); }
        private ImmutableList<PartValue> _partValues = [];

        [Obsolete]
        public ImmutableList<PartBlock> Parts
        {
            set => PartValues = [.. value.Select(p => new PartValue(p))];
        }

        /// <summary>
        /// クラス内のIAnimatableを列挙する。
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IAnimatable> GetAnimatables() => [.. PartValues];
    }
}
