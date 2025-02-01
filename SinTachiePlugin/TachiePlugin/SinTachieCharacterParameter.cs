﻿using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Controls;
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
        [Display(Name = "素材の場所", Description = "立ち絵画像を保存しているフォルダ")]
        [DirectorySelector]
        public string? Directory { get => directory; set => Set(ref directory, value); }
        string? directory = null;
    }
}
