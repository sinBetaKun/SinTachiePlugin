using SinTachiePlugin.CharacterAsset;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Plugin.Tachie;

namespace SinTachiePlugin.Parts
{
    public class SinTachieCharacterParameter : TachieCharacterParameterBase
    {
        /// <summary>
        /// キャラクター編集エリアに表示する立ち絵の設定項目。
        /// [Display]と[DirectorySelector]等のアイテム編集コントロール属性の2つを設定する必要があります。
        /// [DirectorySelector]以外のアイテム編集コントロール属性の一覧はSamplePropertyEditorsプロジェクトを参照してください。
        /// </summary>
        public string? Directory
        {
            set
            {
                if (value is null)
                    return;

                CharacterAssetsData ad_0 = new();
                ad_0.ChangePath(value);
                Assets = [ad_0];
            }
        }

        [Display(Name = "素材の場所", Description = "立ち絵画像を保存しているフォルダ")]
        public ImmutableList<CharacterAssetsData> Assets { get => _assets; set => Set(ref _assets, value); }
        private ImmutableList<CharacterAssetsData> _assets = [];
    }
}
