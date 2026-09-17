using SinTachiePlugin.Enums;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg;
using System.IO;
using YukkuriMovieMaker.Settings;

namespace SinTachiePlugin.PartAnimation.Node.ImageFileNode
{
    /// <summary>
    /// ｱﾆﾒｰｼｮﾝ差分画像群のファイルパスを管理する。
    /// </summary>
    internal class ImageFileNodeManager
    {
        /// <summary>
        /// 代表画像のノード。
        /// <br/>
        /// ここから多次元方向にｱﾆﾒｰｼｮﾝ差分画像が展開される。
        /// </summary>
        public readonly ImageFileNode? Root;

        /// <summary>
        /// 出力値から選出された画像のファイルパス。
        /// <br/>
        /// Updateメソッドによって更新する。
        /// </summary>
        public string Value { get; private set; } = string.Empty;

        public ImageFileNodeManager(string path)
        {
            if (string.IsNullOrEmpty(path))
                // パスが空文字列の場合
                return;

            if (!Path.Exists(path))
                // 指定されたパスを持つファイルがない場合
                return;

            if (FileSettings.Default.FileExtensions.GetFileType(path) != FileType.画像)
                // 指定されたファイルが画像ファイルでない場合
                return;

            // 代表画像のフォルダパス
            string? s_0 = Path.GetDirectoryName(path);

            if (s_0 == null)
                // ディレクトリのパスを取得できなかった場合
                return;

            // パーツ画像をまとめているディレクトリ
            DirectoryInfo dir_0 = new(s_0);

            // 拡張子を取り除いた代表画像のファイル名
            string s_1 = Path.GetFileNameWithoutExtension(path);

            // 代表画像のファイルの拡張子
            string s_2 = Path.GetExtension(path);

            // 代表画像含めた、フォルダ内で発見された差分画像のパス群
            string[] s_a0 =
                [..
                from name2 in dir_0.GetFiles().Select(file => file.Name)
                where FileSettings.Default.FileExtensions.GetFileType(name2) == FileType.画像
                where name2.StartsWith(s_1)
                select name2
                ];

            // ファイル名とその連番を配列に変換したものの辞書
            Dictionary<string, string[]> dic_0 = [];

            // 各ファイル名（代表画像または差分画像のパス）
            foreach (string s_3 in s_a0)
            {
                if (Path.GetFileNameWithoutExtension(s_3) is string fileName)
                {
                    // 代表画像と差分画像それぞれのパスを、姉妹識別子列とともに辞書へ登録
                    dic_0.Add(s_3, fileName.Split(".")[1..]);
                }
            }

            // 最大識別子数（ｱﾆﾒｰｼｮﾝﾚｲﾔｰの次元数）
            int i_0 = dic_0.Values.Select(v => v.Length).Max();
            
            // 代表画像のノード
            ImageFileNode ifn_0 = new(path, 0);
            Root = ifn_0;
            
            // 生成したノードを纏めるリスト
            List<ImageFileNode> ifn_l0 = [ifn_0];

            // 識別子数順に生成リストへ登録
            for (int i_1 = 1; i_1 <= i_0; i_1++)
            {
                foreach (var kv in dic_0.Where(x => x.Value.Length == i_1))
                {
                    // 差分画像のファイルパス
                    string s_3 = kv.Key;
                    
                    // 姉妹識別子群
                    string[] s_a1 = kv.Value;

                    // 最後の姉妹識別子が、負のインデックス(-1)用の記号だった場合
                    if (s_a1.Last() == "_")
                        // そのようなノードには固有の画像ファイルを指すことは許されていない。
                        continue;

                    // 今から作るノードをこのノードの子に追加する
                    ImageFileNode ifn_1 = ifn_0;

                    // ノードが存在するところまで姉妹識別子を順番に読み込む
                    for (int i_2 = 0; i_2 < i_1; i_2++)
                    {
                        // まだノードが存在する場合
                        if (ifn_1.OtherChildren.FirstOrDefault(x => x.Text == s_a1[i_2]) is ImageFileNode ifn_2)
                        {
                            // 参照を子へ移動。
                            ifn_1 = ifn_2;
                        }
                        // ここから先のノードが存在しない場合
                        else
                        {
                            // 以降、最後の直前まで全て"_"である場合
                            if (s_a1.AsSpan()[i_2..^1].Count("_") == i_1 - i_2 - 1)
                            {
                                string s_4 = ifn_1.Path;

                                for (int i_3 = i_2 + 1; i_3 < i_1; i_3++)
                                {
                                    ImageFileNode ifn_3 = new(s_4, i_3) { Text = "_" };
                                    ifn_l0.Add(ifn_3);
                                    // 今は姉妹識別子が数値になっているノードも文字列になっているノードと一緒に纏める。
                                    ifn_1.OtherChildren.Add(ifn_3);
                                    ifn_1 = ifn_3;
                                }

                                ImageFileNode ifn_4 = new(s_3, i_1) { Text = s_a1.Last() };
                                ifn_l0.Add(ifn_4);
                                // 今は姉妹識別子が数値になっているノードも文字列になっているノードと一緒に纏める。
                                ifn_1.OtherChildren.Add(ifn_4);
                            }

                            break;
                        }
                    }
                }
            }

            foreach (ImageFileNode ifn_1 in ifn_l0)
            {
                // 姉妹識別子が数値（正の整数）になっている物をフィルタリング
                var ifn_e0 = ifn_1.OtherChildren.Where(x =>
                {
                    if (int.TryParse(x.Text, out int n1))
                        if (n1 >= 0)
                            return true;

                    return false;
                }).OrderBy(x => int.Parse(x.Text));

                foreach (ImageFileNode _1 in ifn_e0)
                {
                    ifn_1.OtherChildren.Remove(_1);
                    ifn_1.VolumeChildren.Add(_1);
                }
            }

            Value = path;
        }

        public void Update(List<PartAnimationResultA> results, List<PartAnimationNormalizationMode> outers)
        {
            if (Root is not null)
            {
                if (Root.GetValue(results, outers) is string path)
                {
                    Value = path;
                }
            }
        }
    }
}
