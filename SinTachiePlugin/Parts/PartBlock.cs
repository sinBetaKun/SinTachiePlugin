using Newtonsoft.Json;
using SinTachiePlugin.Informations;
using System.IO;
using System.Reflection;
using System.Text;
using Path = System.IO.Path;

namespace SinTachiePlugin.Parts
{
    public class PartBlock : ControlledParamsOfPart
    {
        static string? MakeStpiPath(string path)
        {
            if (!File.Exists(path))
            {
                SinTachieDialog.ShowWarning($"存在しない画像ファイルの{PartInfo.Extension}ファイルパスは作れません。\n({path})");
                return null;
            }
            string? dn = Path.GetDirectoryName(path);
            string? name = Path.GetFileNameWithoutExtension(path);
            if (string.IsNullOrEmpty(dn) || string.IsNullOrEmpty(name))
            {
                SinTachieDialog.ShowWarning($"パスが無効な画像ファイルの{PartInfo.Extension}ファイルパスは作れません。\n({path})");
                return null;
            }
            return Path.Combine(dn, name + "." + PartInfo.Extension);
        }

        [JsonIgnore]
        public bool Selected { get => selected; set => Set(ref selected, value); }
        bool selected = false;

        public PartBlock()
        {
        }

        public PartBlock(string fp, string tag, string[] aborigines)
        {
            if (string.IsNullOrEmpty(TagName)) TagName = tag;
            if (string.IsNullOrEmpty(fp)) return;
            ImagePath = fp;
            if (InputStpi() is PartInfo partInfo)
                if (partInfo.DefaltValues is PartBlock block)
                {
                    CopyFrom(block);
                    if (aborigines.Contains(block.TagName))
                    {
                        var dialog = SinTachieDialog.GetYESorNO(
                            $"デフォルト設定におけるタグ({block.TagName})は、リスト内で既に使われています。" +
                            "\nデフォルト設定のタグを使いますか？" +
                            $"\n（キャンセルした場合、タグは「{tag}」になります。）");
                        if (dialog == DialogResult.Cancel)
                            TagName = tag;
                    }
                }
                    
            ImagePath = fp;
        }

        public PartBlock(PartBlock original)
        {
            CopyFrom(original);
        }

        /// <summary>
        /// 現時点でのSpstInfoのTemplate以外のパラメータを
        /// セットされてる親パーツにおける依存テンプレートとして
        /// 登録（または再登録）したTemplatesを
        /// stptファイルに上書きする。
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void UpdateDefault()
        {
            var info = InputStpi();
            bool overwrited = false;
            if (info != null)
            {
                if (info.DefaltValues != null)
                {
                    var anwer = SinTachieDialog.GetOKorCancel(
                        $"{PartInfo.Extension}ファイルにデフォルト値が設定されています。" +
                        $"\n設定を上書きしますか？"
                        );
                    if (anwer != DialogResult.OK) return;
                    overwrited = true;
                }
            }
            else info = new PartInfo();
            info.DefaltValues = this;
            string str = overwrited ? "上書き" : "保存";
            if (OutputStpi(info)) SinTachieDialog.ShowInformation($"デフォルト値を{str}しました。");
        }

        /// <summary>
        /// 現時点でセットされてる親パーツにおける
        /// 依存テンプレートを消去し
        /// stptファイルに上書きする。
        /// </summary>
        public void DeleteDafault()
        {
            var info = InputStpi();
            if(info == null)
            {
                SinTachieDialog.ShowInformation(
                    $"デフォルト値を記録する{PartInfo.Extension}ファイルはまだ作成されていないので、" +
                    "デフォルト値はまだ設定されていません。");
                return;
            }
            if(info.DefaltValues == null)
            {
                SinTachieDialog.ShowInformation(
                    "デフォルト値はまだ設定されていません。");
                return;
            }
            DialogResult dialogResult = SinTachieDialog.GetOKorCancel(
                $"デフォルト値を削除しますか？\n（{PartInfo.Extension}ファイルは削除されません。）");
            if (dialogResult == DialogResult.OK)
            {
                info.DefaltValues = null;
                OutputStpi(info);
                if (OutputStpi(info)) SinTachieDialog.ShowInformation("デフォルト値を削除しました。");
            }
        }

        public void ReloadDefault()
        {
            string fp = ImagePath;
            string tag = TagName;
            string? path = MakeStpiPath(fp);
            if (string.IsNullOrEmpty(path)) return;
            if (InputStpi() is PartInfo partInfo)
                if (partInfo.DefaltValues is PartBlock block)
                    CopyFrom(block);
            ImagePath = fp;
            Random random = new Random();
            int randomNumber = random.Next(0, 99);
            const int num = 5;
            string str = (randomNumber < num) ? " ☆アローリ☆ " : "リロード";
            SinTachieDialog.ShowInformation($"「{tag}」\nデフォルト値を{str}しました。");
        }

        private PartInfo? InputStpi()
        {
            if(MakeStpiPath(ImagePath) is string stpiPath)
            {
                if (!File.Exists(stpiPath))
                    return null;

                try
                {
                    using (var stream = new FileStream(stpiPath, FileMode.Open))
                    {
                        using (var sr = new StreamReader(stream))
                        {

                            if (JsonConvert.DeserializeObject<PartInfo>(sr.ReadToEnd(), GetJsonSetting) is PartInfo info)
                            {
                                return info;
                            }
                            else
                            {
                                SinTachieDialog.ShowWarning($"{PartInfo.Extension}ファイルから情報を取得できませんでした。");
                                return null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SinTachieDialog.ShowWarning($"{PartInfo.Extension}ファイルの読み込みに失敗しました。\n" + ex.Message);
                    return null;
                }
            }
            else return null;
        }

        /// <summary>
        /// Templates を stptファイルに出力
        /// </summary>
        private bool OutputStpi(PartInfo partInfo)
        {
            if (MakeStpiPath(ImagePath) is string stpiPath)
            {
                var path = ImagePath;
                ImagePath = string.Empty;
                try
                {
                    string stpi = JsonConvert.SerializeObject(partInfo, Formatting.Indented, GetJsonSetting);
                    using (var sw = new StreamWriter(stpiPath, false, Encoding.UTF8))
                        sw.Write(stpi);
                }
                catch (Exception e)
                {
                    SinTachieDialog.ShowError(new("stpiファイルの出力時にエラーが発生しました。\n" + e.Message));
                    return false;
                }
                ImagePath = path;
                return true;
            }
            return false;
        }
    }
}
