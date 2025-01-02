using SinTachiePlugin.Informations;
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
            /*if (!tachieItemDescriptions.Any())
            {
                yield break;
            }

            ITachieItemParameter itemParameter = tachieItemDescriptions.First().ItemParameter;
            if (!(itemParameter is SinTachieItemParameter ip) || faceItemDescriptions == null || !faceItemDescriptions.Any())
            {
                yield break;
            }

            string characterName = tachieItemDescriptions.First().CharacterName;
            foreach (TachieVoiceItemExoDescription voiceDescription in voiceDescriptions)
            {
                ExoItem clone = voiceDescription.ExoItem.GetClone();
                clone.SubLayer = 2;
                clone.Filters.AddRange(
                [
                "_name=テキスト\r\ntext=" + GetStringForExo(characterName + " / 口パク") + "\r\n",
                $"_name=アニメーション効果\r\nname=口パク@動く立ち絵\r\nparam=file=\"{voiceDescription?.VoiceFilePath?.Replace("\\", "\\\\")}\";\r\n",
                "_name=標準描画\r\nX=0.0\r\nY=0.0\r\nZ=0.0\r\n拡大率=100.00\r\n透明度=100.0\r\n回転=0.00\r\nblend=0\r\n"
                ]);
                yield return clone;
            }*/
        }

        private static string GetStringForExo(string text)
        {
            return string.Concat(text.Select(delegate (char c)
            {
                byte[] bytes = BitConverter.GetBytes(c);
                return $"{bytes[0]:x2}{bytes[1]:x2}";
            }).Concat(Enumerable.Repeat("0000", 1024 - text.Length)));
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
