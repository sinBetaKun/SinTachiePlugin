﻿using SinTachiePlugin.Informations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Tachie;

namespace SinTachiePlugin.Parts
{
    public class SinTachiePlugin : ITachiePlugin
    {
        /// <summary>
        /// プラグインの名前
        /// </summary>
        public string Name => PluginInfo.Title;

        /// <summary>
        /// 俺ちゃんの名前
        /// </summary>
        public PluginDetailsAttribute Details => new() { AuthorName = "sinβ" };

        /// <summary>
        /// キャラクターに設定する立ち絵のパラメーター
        /// </summary>
        /// <returns></returns>
        public ITachieCharacterParameter CreateCharacterParameter() => new SinTachieCharacterParameter();

        /// <summary>
        /// 立ち絵アイテムに設定する立ち絵のパラメーター
        /// </summary>
        /// <returns></returns>
        public ITachieItemParameter CreateItemParameter() => new SinTachieItemParameter();

        /// <summary>
        /// ボイスアイテムや表情アイテムに設定するパラメーター
        /// </summary>
        /// <returns></returns>
        public ITachieFaceParameter CreateFaceParameter() => new SinTachieFaceParameter();

        /// <summary>
        /// Exoアイテムを作成する
        /// </summary>
        /// <param name="FPS">動画のフレームレート</param>
        /// <param name="tachieItemDescriptions">立ち絵アイテムの一覧</param>
        /// <param name="faceItemDescriptions">表情アイテムの一覧</param>
        /// <param name="voiceDescriptions">ボイスアイテムの一覧</param>
        /// <returns></returns>
        public IEnumerable<ExoItem> CreateExoItems(int FPS, IEnumerable<TachieItemExoDescription> tachieItemDescriptions, IEnumerable<TachieFaceItemExoDescription> faceItemDescriptions, IEnumerable<TachieVoiceItemExoDescription> voiceDescriptions)
        {
            return [];
        }

        /// <summary>
        /// AviUtl用のスクリプトファイルを出力するかどうか
        /// </summary>
        public bool HasScriptFile => false;
        /// <summary>
        /// AviUtl用のスクリプトファイルを出力する
        /// </summary>
        /// <param name="scriptDirectoryPath">出力先のフォルダ</param>
        public void CreateScriptFile(string scriptDirectoryPath)
        {

        }

        /// <summary>
        /// 立ち絵ソースを作成する
        /// </summary>
        /// <param name="devices">デバイス一覧</param>
        /// <returns>立ち絵一覧</returns>
        public ITachieSource CreateTachieSource(IGraphicsDevicesAndContext devices)
        {
            return new SinTachieSource(devices);
        }
    }
}
