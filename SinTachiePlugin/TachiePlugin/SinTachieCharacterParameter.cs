using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Plugin.Tachie;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Parts
{
    public class SinTachieCharacterParameter : TachieCharacterParameterBase, IFileItem
    {
        /// <summary>
        /// キャラクター編集エリアに表示する立ち絵の設定項目。
        /// [Display]と[DirectorySelector]等のアイテム編集コントロール属性の2つを設定する必要があります。
        /// [DirectorySelector]以外のアイテム編集コントロール属性の一覧はSamplePropertyEditorsプロジェクトを参照してください。
        /// </summary>
        [Display(Name = "素材の場所", Description = "立ち絵画像を保存しているフォルダ")]
        [DirectorySelector]
        public string Directory { get; set => Set(ref field, value); } = string.Empty;

        public IEnumerable<string> GetFiles() => [Directory];

        public void ReplaceFile(string from, string to)
        {
            if (Directory == from)
                Directory = to;
        }

        public IEnumerable<TimelineResource> GetResources()
        {
            if (TimelineResource.TryParseFromPath(Directory, TimelineResourceType.Tachie, out TimelineResource? tr_0))
                yield return tr_0;
        }
    }
}
